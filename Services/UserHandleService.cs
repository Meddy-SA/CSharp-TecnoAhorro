using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using TecnoCredito.Models.Authentication;
using TecnoCredito.Models.Authentication.DTOs;
using TecnoCredito.Models.DTOs;
using TecnoCredito.Models.Enums;
using TecnoCredito.Services.Interfaces;

namespace TecnoCredito.Services;

public class UserHandleService(
  SignInManager<AppUser> signInManager,
  UserManager<AppUser> userManager,
  IUserStore<AppUser> userStore,
  RoleManager<IdentityRole> roleManager,
  IUserTwoFactorTokenProvider<AppUser> twoFactorTokenProvider,
  IEmailSender emailSender,
  IClaim security,
  IConfiguration configuration,
  ISysMenu sysMenu) : IUserHandle
{
  private readonly SignInManager<AppUser> signInManager = signInManager;
  private readonly UserManager<AppUser> userManager = userManager;
  private readonly IUserStore<AppUser> userStore = userStore;
  private readonly RoleManager<IdentityRole> roleManager = roleManager;
  private readonly IClaim security = security;
  private readonly IUserTwoFactorTokenProvider<AppUser> twoFactorTokenProvider = twoFactorTokenProvider;
  private readonly IEmailSender emailSender = emailSender;
  public IList<AuthenticationScheme> ExternalLogins { get; set; } = null!;
  private readonly IUserEmailStore<AppUser> emailStore = GetEmailStore(userManager, userStore);
  private readonly ISysMenu sysMenu = sysMenu;
  private readonly string baseURL = configuration.GetSection("Service")["Front"] ?? "";
  private static string TitleEmail => "Tecno Ahorro - Credito";

  public async Task<ResponseDTO<AppUserDTO>> Login(LoginDTO loginDTO)
  {
    ResponseDTO<AppUserDTO> response = new()
    {
      Result = new()
      {
        Email = loginDTO.User,
        FirstName = "",
        LastName = "",
        Password = ""
      },
      Success = false
    };
    List<IdentityError> messages = default!;
    AppUser? userApp = null!;

    try
    {
      bool userisEmail = loginDTO.User.Contains('@');
      if (userisEmail)
      {
        userApp = await userManager.FindByEmailAsync(loginDTO.User);
      }
      else
      {
        userApp = await userManager.FindByNameAsync(loginDTO.User);
      }

      if (userApp == null)
      {
        messages = [GetError("InvalidUser")];
        response.Error = "El usuario o contraseña son incorrectos.";
        response.Result.Messages = messages;
        return response;
      }

      // Si el usuario existe, verifico las credenciales.
      var result = await signInManager.PasswordSignInAsync(userApp, loginDTO.Password, false, lockoutOnFailure: false);

      if (result.Succeeded)
      {
        if (userisEmail && !userApp.EmailConfirmed)
        {
          messages = [GetError("EmailNotConfirmed")];
          response.Error = "Usuario Debe Confirmar Correo.";
          response.Result.Messages = messages;
          return response;
        }
        // Obtengo los roles del usuario para enviar al token.
        var roles = await userManager.GetRolesAsync(userApp);
        var token = security.GenerateToken(userApp, roles);

        response.Result = new AppUserDTO
        {
          UserName = userApp.UserName!,
          Email = userApp.Email!,
          FullName = userApp.FullName(),
          FirstName = userApp.FirstName,
          LastName = userApp.LastName,
          Password = "",
          Token = token,
        };
        response.Error = "Usuario Conectado";
        response.Success = true;
        return response;
      }

      if (result.RequiresTwoFactor)
      {
        var resultCanGenerate = await twoFactorTokenProvider.CanGenerateTwoFactorTokenAsync(userManager, userApp);
        messages = resultCanGenerate ? [GetError("LoginWith2fa")] : [GetError("Invalid2FA")];

        response.Error = resultCanGenerate ? "Requiere autenticación de dos factores." : "Se requiere primero verificar el correo electrónico.";
        await twoFactorTokenProvider.GenerateAsync("Email", userManager, userApp);
        response.Result.Messages = messages;
        return response;
      }

      if (result.IsLockedOut)
      {
        response.Error = "La cuenta está bloqueada.";
        messages = [GetError("Lockout")];
      }
      else if (result.IsNotAllowed)
      {
        response.Error = "Validación de correo electrónico requerida.";
        messages = [GetError("EmailRequired")];
      }
      else
      {
        response.Error = "Intento de inicio de sesión no válido.";
        messages = [GetError("Invalid")];
      }
    }
    catch (Exception ex)
    {
      response.Error = $"Error al intentar iniciar sesión: {ex.Message}";
      response.Success = false;
      messages = [GetError("ServerError")];
    }

    response.Result.Messages = messages;
    return response;
  }

  public async Task<ResponseDTO<AppUserDTO>> LoginWith2FA(LoginDTO loginDTO)
  {
    ResponseDTO<AppUserDTO> response = new();
    AppUser? userWith2fa = null!;
    bool userisEmail = loginDTO.User.Contains('@');
    if (userisEmail)
    {
      userWith2fa = await userManager.FindByEmailAsync(loginDTO.User);
    }
    else
    {
      userWith2fa = await userManager.FindByNameAsync(loginDTO.User);
    }
    if (userWith2fa == null)
    {
      response.Error = "El usuario no es válido.";
      return response;
    }
    var result = await twoFactorTokenProvider.ValidateAsync("Email", loginDTO.Password, userManager, userWith2fa);
    response.Success = result;

    if (result)
    {
      var roles = await userManager.GetRolesAsync(userWith2fa);
      var token = security.GenerateToken(userWith2fa, roles);
      var userDTO = new AppUserDTO
      {
        UserName = userWith2fa.UserName!,
        Email = userWith2fa.Email!,
        FullName = userWith2fa.FullName(),
        FirstName = userWith2fa.FirstName,
        LastName = userWith2fa.LastName,
        Password = "",
        Token = token,
      };
      response.Result = userDTO;
      response.Error = "Autenticación correcta";
    }
    else
    {
      response.Result = new();
      response.Error = "El código ingresado no es correcto";
      response.Result.Messages = new List<IdentityError> { GetError("InvalidCode2FA") };
    }

    return response;
  }

  public async Task<ResponseDTO<AppUserDTO>> CreateUser(AppUserDTO userDTO, string returnUrl = null!)
  {
    ResponseDTO<AppUserDTO> responseDTO = new() { Success = false };
    ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    var user = Activator.CreateInstance<AppUser>();

    await userStore.SetUserNameAsync(user, userDTO.UserName, CancellationToken.None);
    await emailStore.SetEmailAsync(user, userDTO.Email, CancellationToken.None);

    user.FirstName = userDTO.FirstName;
    user.LastName = userDTO.LastName;

    var result = await userManager.CreateAsync(user, userDTO.Password);
    if (result.Succeeded)
    {
      userDTO.Token = security.GenerateToken(user, null); // Genera token para usuario nuevo.

      var userId = await userManager.GetUserIdAsync(user);
      userDTO.UserId = userId;

      var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
      code = WebEncoders.Base64UrlEncode(System.Text.Encoding.UTF8.GetBytes(code));

      var emailBody = BodyEmail(user.Id, code);

      await emailSender.SendEmailAsync(userDTO.Email, "Confirmar Correo", emailBody, user.FullName());

      if (userManager.Options.SignIn.RequireConfirmedAccount)
      {
        userDTO.Messages = new List<IdentityError> { GetError("EmailNotConfirmed") };
      }
    }
    else
    {
      userDTO.Messages = result.Errors.ToList();
    }

    responseDTO.Success = result.Succeeded;
    userDTO.Password = "";
    responseDTO.Result = userDTO;

    return responseDTO;
  }

  public async Task<ResponseDTO<string>> ValidateEmail(ValidateEmailDTO validateEmailDTO)
  {
    // UNDONE: Falta revisar.
    ResponseDTO<string> responseDTO = new();
    if (validateEmailDTO.UserId == null || validateEmailDTO.Code == null)
    {
      responseDTO.Success = false;
      responseDTO.Error = "Error confirmando tu correo electrónico";
    }
    else
    {
      var user = await userManager.FindByIdAsync(validateEmailDTO.UserId);
      if (user == null)
      {
        responseDTO.Success = false;
        responseDTO.Error = $"No se puede cargar el usuario con ID: {validateEmailDTO.UserId}";
      }
      else
      {
        var code = System.Text.Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(validateEmailDTO.Code));
        var result = await userManager.ConfirmEmailAsync(user, code);
        responseDTO.Success = result.Succeeded;
        responseDTO.Error = result.Succeeded ?
          "Gracias por confirmar tu correo electrónico" :
          "Error confirmando tu correo electrónico";
      }

    }
    responseDTO.Result = "";
    return responseDTO;
  }

  public async Task<ResponseDTO<string>> ReSendEmailValidate(string email)
  {
    // UNDONE: Falta revisar.
    ResponseDTO<string> responseDTO = new()
    {
      Error = "Correo de verificación enviado. Por favor, revisa tu correo electrónico."
    };
    var user = await userManager.FindByEmailAsync(email);
    if (user == null)
    {
      responseDTO.Success = false;
      responseDTO.Result = "Usuario no encontrado.";
    }
    else
    {
      var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
      code = WebEncoders.Base64UrlEncode(System.Text.Encoding.UTF8.GetBytes(code));

      var emailBody = BodyEmail(user.Id, code);
      await emailSender.SendEmailAsync(email, "Confirmar Correo", emailBody, user.FullName(), true);
      responseDTO.Success = true;
      responseDTO.Result = user.Id;
    }
    return responseDTO;
  }

  public async Task<ResponseDTO<PersonalDataDTO>> GetPersonalData(string userId)
  {
    // UNDONE: Falta revisar. No hacerlo por id.
    ResponseDTO<PersonalDataDTO> responseDTO = new();
    var user = await userManager.FindByIdAsync(userId);
    if (user == null)
    {
      responseDTO.Success = false;
      responseDTO.Error = $"No se puede cargar el usuario con ID: {userId}";
      responseDTO.Result = new PersonalDataDTO();
    }
    else
    {
      responseDTO.Success = true;
      responseDTO.Error = "Usuario cargado correctamente";
      responseDTO.Result = new PersonalDataDTO()
      {
        FirstName = user.FirstName,
        MiddleName = user.MiddleName ?? "",
        LastName = user.LastName,
        SecondSurName = user.SecondSurname ?? "",
        LoginWith2FA = user.TwoFactorEnabled,
        Avatar = user.ProfilePictureBase64
      };
    }
    return responseDTO;
  }

  public async Task<ResponseDTO<string>> UpdatePersonalData(string userId, PersonalDataDTO personalData)
  {
    // UNDONE: Falta revisar. QUe no sea por ID.
    ResponseDTO<string> responseDTO = new()
    {
      Error = "Datos actualizados."
    };
    var user = await userManager.FindByIdAsync(userId);
    if (user == null)
    {
      responseDTO.Success = false;
      responseDTO.Result = "Usuario no encontrado.";
    }
    else
    {
      user.FirstName = personalData.FirstName;
      user.MiddleName = personalData.MiddleName;
      user.LastName = personalData.LastName;
      user.SecondSurname = personalData.SecondSurName;
      user.TwoFactorEnabled = personalData.LoginWith2FA;
      if (personalData.Avatar != null)
      {
        user.ProfilePicture = Convert.FromBase64String(personalData.Avatar);
      }
      await userManager.UpdateAsync(user);
    }
    return responseDTO;
  }

  private string BodyEmail(string userId, string code)
  {

    var callbackUrl = $"{baseURL}verify-email/validate-code?userId={userId}&code={code}";

    var htmlBody = $@"
            <html>
                <body style='background-color: #ffffe0;'>
                    <div style='text-align: center;'>
                        <img src='cid:logo.png' alt='{TitleEmail}' style='max-width: 300px;' />
                        <h1 style='font-size: 24px; color: #333;'>{TitleEmail}</h1>
                        <p style='font-size: 18px;'>Por favor, confirma tu cuenta haciendo clic en el siguiente botón:</p>
                        <a href='{System.Text.Encodings.Web.HtmlEncoder.Default.Encode(callbackUrl)}' style='
                          display: inline-block; font-weight: bold; text-align: center;
                          text-decoration: none; background-color: #4CAF50;
                          color: white; padding: 10px 20px; margin: 10px auto; border-radius: 5px; cursor: pointer;'>
                          Confirmar Cuenta
                        </a>
                        <p style='font-size: 18px; margin-top: 1.5rem;'>
                          ó copiar el código e ingresarlo manualmente:
                        </p>
                        <div style='background-color: #ffffcc; padding: 10px; border-radius: 5px; font-size: 24px;'>
                            {code}
                        </div>
                    </div>
                </body>
            </html>
        ";
    return htmlBody;
  }

  private static IUserEmailStore<AppUser> GetEmailStore(UserManager<AppUser> userManager, IUserStore<AppUser> userStore)
  {
    if (!userManager.SupportsUserEmail)
    {
      throw new NotSupportedException("The default UI requires a user store with email support.");
    }
    return (IUserEmailStore<AppUser>)userStore;
  }

  private static IdentityError GetError(string code)
  {
    var customErrors = new List<CustomErrorDTO> {
      new("EmailNotConfirmed", "Debe confirmar email para tener acceso a todas las funciones."),
      new("LoginWith2fa", "Se requiere autenticación de dos factores."),
      new("InvalidCode2FA", "El código es incorrecto."),
      new("Lockout", "La cuenta está bloqueada."),
      new("EmailRequired", "Se requiere la validación de correo electrónico para iniciar sesión."),
      new("Invalid", "Intento de inicio de sesión no válido."),
      new("Invalid2FA", "Para autenticarse, debe tener verificado el correo electrónico."),
      new("ServerError", "Error interno del servidor."),
      new("InvalidUser", "El usuario o contraseña son incorrectos"),
    };
    var error = customErrors.FirstOrDefault(e => e.Code == code)!;
    return new()
    {
      Code = error.Code,
      Description = error.Description
    };
  }

  public async Task EnsureRolesAsync()
  {
    foreach (var rol in Enum.GetValues<RolesEnum>())
    {
      var nameRol = rol.ToString();
      var alreadyExists = await roleManager.RoleExistsAsync(nameRol);
      if (!alreadyExists)
      {
        await roleManager.CreateAsync(new IdentityRole(nameRol));
      }
    }
  }

  public async Task EnsureTestUserAsync()
  {
    var testUser = await userManager.FindByEmailAsync("leonardoillanez@meddyai.com");
    if (testUser == null)
    {
      testUser = new AppUser
      {
        FirstName = "Leonardo",
        LastName = "Illanez",
        UserName = "leonardoilla777",
        EmailConfirmed = true,
        Email = "leonardoillanez@meddyai.com",
      };
      await userManager.CreateAsync(testUser, "Password123!");
    }
    await userManager.AddToRoleAsync(testUser, "SuperAdmin");
  }

  public async Task EnsureTestMenuAsync()
  {
    string categoryName = "Parametros";
    List<RolesEnum> roles = [RolesEnum.SuperAdmin, RolesEnum.Admin];
    await sysMenu.AddCategoryAsync(categoryName, roles);
  }
}
