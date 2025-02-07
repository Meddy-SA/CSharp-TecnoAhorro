using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using TecnoCredito.Contexts;
using TecnoCredito.Helpers;
using TecnoCredito.Models.Authentication;
using TecnoCredito.Models.Authentication.DTOs;
using TecnoCredito.Models.DTOs;
using TecnoCredito.Models.Enums;
using TecnoCredito.Services.Interfaces;

namespace TecnoCredito.Services;

public class AuthenticationService(
    SignInManager<AppUser> signInManager,
    UserManager<AppUser> userManager,
    IUserStore<AppUser> userStore,
    RoleManager<AppRole> roleManager,
    IUserTwoFactorTokenProvider<AppUser> twoFactorTokenProvider,
    IEmailSender emailSender,
    IClaim security,
    IConfiguration configuration,
    ILogger<AuthenticationService> logger,
    ISysMenu sysMenu,
    Context context,
    IMapper mapper
) : IAuthenticationHandle
{
    private readonly SignInManager<AppUser> signInManager = signInManager;
    private readonly UserManager<AppUser> userManager = userManager;
    private readonly IUserStore<AppUser> userStore = userStore;
    private readonly RoleManager<AppRole> roleManager = roleManager;
    private readonly IClaim security = security;
    private readonly IUserTwoFactorTokenProvider<AppUser> twoFactorTokenProvider =
        twoFactorTokenProvider;
    private readonly IEmailSender emailSender = emailSender;
    public IList<AuthenticationScheme> ExternalLogins { get; set; } = null!;
    private readonly IUserEmailStore<AppUser> emailStore = GetEmailStore(userManager, userStore);
    private readonly ISysMenu sysMenu = sysMenu;
    private readonly IConfiguration configuration = configuration;
    private readonly ILogger<AuthenticationService> logger = logger;
    private readonly Context context = context;
    private readonly IMapper mapper = mapper;

    private readonly string baseURL = configuration.GetSection("App")["Cors"] ?? "";
    private readonly string titleEmail =
        configuration.GetSection("EmailSender")["Title"] ?? "Tecno Ahorro - Credito";

    public async Task<ResponseDTO<AppUserDTO>> Login(LoginDTO loginDTO)
    {
        List<IdentityError> messages = default!;
        AppUser? userApp = null!;
        string errors = "",
            msg = "";

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
                errors = string.Join(", ", messages.Select(x => x.Description));
                return ResponseDTO<AppUserDTO>.Fail(
                    errors,
                    "El usuario o contraseña son incorrectos."
                );
            }

            // Si el usuario existe, verifico las credenciales.
            var result = await signInManager.PasswordSignInAsync(
                userApp,
                loginDTO.Password,
                false,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                if (userisEmail && !userApp.EmailConfirmed)
                {
                    messages = [GetError("EmailNotConfirmed")];
                    errors = string.Join(", ", messages.Select(x => x.Description));
                    return ResponseDTO<AppUserDTO>.Fail(errors, "Usuario Debe Confirmar Correo.");
                }
                // Obtengo los roles del usuario para enviar al token.
                var roles = await userManager.GetRolesAsync(userApp);
                var token = security.GenerateToken(userApp, roles);
                var rolesEnum = EnumExtensions.GetListRoleForName(roles);
                var menu = await sysMenu.GetMenuForRole(rolesEnum);

                var appUserDTO = new AppUserDTO
                {
                    UserName = userApp.UserName!,
                    Email = userApp.Email!,
                    FullName = userApp.FullName(),
                    FirstName = userApp.FirstName,
                    LastName = userApp.LastName,
                    Password = "",
                    Token = token,
                    Menu = menu.IsSuccess ? menu.Result : null,
                };
                return ResponseDTO<AppUserDTO>.Success(appUserDTO, "Usuario Conectado");
            }

            if (result.RequiresTwoFactor)
            {
                var resultCanGenerate = await twoFactorTokenProvider.CanGenerateTwoFactorTokenAsync(
                    userManager,
                    userApp
                );
                messages = resultCanGenerate
                    ? [GetError("LoginWith2fa")]
                    : [GetError("Invalid2FA")];
                errors = string.Join(", ", messages.Select(x => x.Description));
                msg = resultCanGenerate
                    ? "Requiere autenticación de dos factores."
                    : "Se requiere primero verificar el correo electrónico.";
                await twoFactorTokenProvider.GenerateAsync("Email", userManager, userApp);
                return ResponseDTO<AppUserDTO>.Fail(errors, msg);
            }

            if (result.IsLockedOut)
            {
                msg = "La cuenta está bloqueada.";
                messages = [GetError("Lockout")];
            }
            else if (result.IsNotAllowed)
            {
                msg = "Validación de correo electrónico requerida.";
                messages = [GetError("EmailRequired")];
            }
            else
            {
                msg = "Intento de inicio de sesión no válido.";
                messages = [GetError("Invalid")];
            }
            return ResponseDTO<AppUserDTO>.Fail(errors, msg);
        }
        catch (Exception ex)
        {
            messages = [GetError("ServerError")];
            errors = string.Join(", ", messages.Select(x => x.Description));
            return ResponseDTO<AppUserDTO>.Fail(
                errors,
                $"Error al intentar iniciar sesión: {ex.Message}"
            );
        }
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
        var result = await twoFactorTokenProvider.ValidateAsync(
            "Email",
            loginDTO.Password,
            userManager,
            userWith2fa
        );
        response.IsSuccess = result;

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

    private async Task<ResponseDTO<PreRegisterDTO>> PreRegister(PreRegisterDTO preRegister)
    {
        try
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(preRegister.Email))
            {
                return ResponseDTO<PreRegisterDTO>.Failure("El email es requerido.");
            }

            if (string.IsNullOrWhiteSpace(preRegister.FullName))
            {
                return ResponseDTO<PreRegisterDTO>.Failure("El nombre completo es requerido.");
            }

            if (!preRegister.TermsAccepted)
            {
                return ResponseDTO<PreRegisterDTO>.Failure(
                    "Debe aceptar los términos y condiciones."
                );
            }

            // Verificar si el email ya existe en PreRegister
            var existingPreRegister = await context.PreRegisters.FirstOrDefaultAsync(p =>
                p.Email.ToLower() == preRegister.Email.ToLower()
            );

            if (existingPreRegister != null)
            {
                // Actualizar registro existente
                existingPreRegister.FullName = preRegister.FullName;
                existingPreRegister.Country = preRegister.Country;
                existingPreRegister.TermsAccepted = preRegister.TermsAccepted;
                existingPreRegister.EmailUpdated = true;

                context.PreRegisters.Update(existingPreRegister);
                await context.SaveChangesAsync();

                var existPreRegisterDTO = mapper.Map<PreRegisterDTO>(existingPreRegister);

                return ResponseDTO<PreRegisterDTO>.Success(
                    existPreRegisterDTO,
                    "Registro previo actualizado exitosamente."
                );
            }

            // Verificar si el email ya existe en AppUser
            var existingUser = await userManager.FindByEmailAsync(preRegister.Email);
            if (existingUser != null)
            {
                return ResponseDTO<PreRegisterDTO>.Failure(
                    "El email ya está registrado como usuario.",
                    "Ya existe una cuenta con este email."
                );
            }

            // Crear nuevo pre-registro
            var newPreRegister = new PreRegister
            {
                Email = preRegister.Email.ToLower(),
                FullName = preRegister.FullName,
                Country = preRegister.Country,
                TermsAccepted = preRegister.TermsAccepted,
                EmailUpdated = true,
            };

            await context.PreRegisters.AddAsync(newPreRegister);
            await context.SaveChangesAsync();

            var newPreRegisterDTO = mapper.Map<PreRegisterDTO>(newPreRegister);

            return ResponseDTO<PreRegisterDTO>.Success(
                newPreRegisterDTO,
                "Pre-registro creado exitosamente."
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error al procesar el pre-registro: {ex.Message}");
            return ResponseDTO<PreRegisterDTO>.Failure(
                ex.Message,
                "Ha ocurrido un error al procesar el pre-registro."
            );
        }
    }

    public async Task<ResponseDTO<AppUserDTO>> CreateUser(
        PreRegisterDTO preRegisterDto,
        string returnUrl = null!
    )
    {
        try
        {
            // Intentar pre-registrar al usuario
            var preRegisterResponse = await PreRegister(preRegisterDto);
            if (!preRegisterResponse.IsSuccess)
            {
                return ResponseDTO<AppUserDTO>.Failure(
                    preRegisterResponse.Message,
                    preRegisterResponse.Error
                );
            }

            // Obtener datos del pre-registro exitoso
            var preRegister = preRegisterResponse.Result;

            // Parsear el nombre completo usando el helper
            var parsedName = NameParserHelper.ParseFullName(preRegister?.FullName ?? "Sin Nombre");

            ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Crear instancia de AppUser
            var user = Activator.CreateInstance<AppUser>();

            await userStore.SetUserNameAsync(user, preRegisterDto.Email, CancellationToken.None);
            await emailStore.SetEmailAsync(user, preRegisterDto.Email, CancellationToken.None);
            user.FirstName = parsedName.FirstName;
            user.LastName = parsedName.LastName;
            user.MiddleName = parsedName.MiddleName;
            user.SecondSurname = parsedName.SecondSurname;
            user.TwoFactorEnabled = false;

            var result = await userManager.CreateAsync(user, preRegisterDto.Password);
            if (
                !result.Succeeded
                || user == null
                || string.IsNullOrEmpty(user.Email)
                || string.IsNullOrEmpty(user.UserName)
            )
            {
                return ResponseDTO<AppUserDTO>.Failure(
                    "Error al crear el usuario",
                    result.Errors.FirstOrDefault()?.Description ?? "Error desconocido"
                );
            }

            // Generar token de confirmación de email
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedCode = WebEncoders.Base64UrlEncode(System.Text.Encoding.UTF8.GetBytes(code));

            // Preparar el cuerpo del email
            var userId = await userManager.GetUserIdAsync(user);
            var emailBody = BodyEmail(user.Id, encodedCode);

            // Enviar email de confirmación
            await emailSender.SendEmailAsync(user.Email, "Confirmar Correo", emailBody, true);

            // Generar token JWT para el usuario
            var jwtToken = security.GenerateToken(user, null);

            // Preparar la respuesta
            var appUserDto = new AppUserDTO
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.ToFullName(),
                Token = jwtToken,
                Messages = userManager.Options.SignIn.RequireConfirmedAccount
                    ? new List<IdentityError> { GetError("EmailNotConfirmed") }
                    : new List<IdentityError>(),
            };

            return ResponseDTO<AppUserDTO>.Success(
                appUserDto,
                "Usuario creado exitosamente. Por favor, verifica tu correo electrónico."
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al crear usuario");
            return ResponseDTO<AppUserDTO>.Failure(
                "Error interno del servidor",
                "Ha ocurrido un error al procesar la creación del usuario."
            );
        }
    }

    public async Task<ResponseDTO<string>> ValidateEmail(ValidateEmailDTO validateEmailDTO)
    {
        // UNDONE: Falta revisar.
        ResponseDTO<string> responseDTO = new();
        if (validateEmailDTO.UserId == null || validateEmailDTO.Code == null)
        {
            responseDTO.IsSuccess = false;
            responseDTO.Error = "Error confirmando tu correo electrónico";
        }
        else
        {
            var user = await userManager.FindByIdAsync(validateEmailDTO.UserId);
            if (user == null)
            {
                responseDTO.IsSuccess = false;
                responseDTO.Error =
                    $"No se puede cargar el usuario con ID: {validateEmailDTO.UserId}";
            }
            else
            {
                var code = System.Text.Encoding.UTF8.GetString(
                    WebEncoders.Base64UrlDecode(validateEmailDTO.Code)
                );
                var result = await userManager.ConfirmEmailAsync(user, code);
                responseDTO.IsSuccess = result.Succeeded;
                responseDTO.Error = result.Succeeded
                    ? "Gracias por confirmar tu correo electrónico"
                    : "Error confirmando tu correo electrónico";
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
            Error = "Correo de verificación enviado. Por favor, revisa tu correo electrónico.",
        };
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            responseDTO.IsSuccess = false;
            responseDTO.Result = "Usuario no encontrado.";
        }
        else
        {
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(System.Text.Encoding.UTF8.GetBytes(code));

            var emailBody = BodyEmail(user.Id, code);
            await emailSender.SendEmailAsync(email, "Confirmar Correo", emailBody, true);
            responseDTO.IsSuccess = true;
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
            responseDTO.IsSuccess = false;
            responseDTO.Error = $"No se puede cargar el usuario con ID: {userId}";
            responseDTO.Result = new PersonalDataDTO();
        }
        else
        {
            responseDTO.IsSuccess = true;
            responseDTO.Error = "Usuario cargado correctamente";
            responseDTO.Result = new PersonalDataDTO()
            {
                FirstName = user.FirstName,
                MiddleName = user.MiddleName ?? "",
                LastName = user.LastName,
                SecondSurName = user.SecondSurname ?? "",
                LoginWith2FA = user.TwoFactorEnabled,
                Avatar = user.ProfilePictureBase64,
            };
        }
        return responseDTO;
    }

    public async Task<ResponseDTO<string>> UpdatePersonalData(
        string userId,
        PersonalDataDTO personalData
    )
    {
        // UNDONE: Falta revisar. QUe no sea por ID.
        ResponseDTO<string> responseDTO = new() { Error = "Datos actualizados." };
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            responseDTO.IsSuccess = false;
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

        var htmlBody =
            $@"
            <html>
                <body style='background-color: #ffffe0;'>
                    <div style='text-align: center;'>
                        <img src='cid:logo.png' alt='{titleEmail}' style='max-width: 300px;' />
                        <h1 style='font-size: 24px; color: #333;'>{titleEmail}</h1>
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

    private static IUserEmailStore<AppUser> GetEmailStore(
        UserManager<AppUser> userManager,
        IUserStore<AppUser> userStore
    )
    {
        if (!userManager.SupportsUserEmail)
        {
            throw new NotSupportedException(
                "The default UI requires a user store with email support."
            );
        }
        return (IUserEmailStore<AppUser>)userStore;
    }

    private static IdentityError GetError(string code)
    {
        var customErrors = new List<CustomErrorDTO>
        {
            new(
                "EmailNotConfirmed",
                "Debe confirmar email para tener acceso a todas las funciones."
            ),
            new("LoginWith2fa", "Se requiere autenticación de dos factores."),
            new("InvalidCode2FA", "El código es incorrecto."),
            new("Lockout", "La cuenta está bloqueada."),
            new(
                "EmailRequired",
                "Se requiere la validación de correo electrónico para iniciar sesión."
            ),
            new("Invalid", "Intento de inicio de sesión no válido."),
            new("Invalid2FA", "Para autenticarse, debe tener verificado el correo electrónico."),
            new("ServerError", "Error interno del servidor."),
            new("InvalidUser", "El usuario o contraseña son incorrectos"),
        };
        var error = customErrors.FirstOrDefault(e => e.Code == code)!;
        return new() { Code = error.Code, Description = error.Description };
    }

    public async Task EnsureTestMenuAsync()
    {
        string categoryName = "Parametros";
        List<RolesEnum> roles = [RolesEnum.SuperAdmin, RolesEnum.Admin];
        await sysMenu.AddCategoryAsync(categoryName, roles);
    }
}
