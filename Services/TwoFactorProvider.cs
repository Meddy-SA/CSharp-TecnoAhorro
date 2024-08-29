using TecnoCredito.Models.Authentication;
using TecnoCredito.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace TecnoCredito.Services;

public class TwoFactorTokenProvider : IUserTwoFactorTokenProvider<AppUser>
{
  public static string ProviderName => "CustomTwoFactorProvider";
  private readonly UserManager<AppUser> userManager;
  private readonly IEmailSender emailService;
  private static string TitleApp => "Tecno Ahorro";

  public TwoFactorTokenProvider(UserManager<AppUser> userManager, IEmailSender emailService)
  {
    this.userManager = userManager;
    this.emailService = emailService;
  }

  public async Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<AppUser> manager, AppUser user)
  {
    var providers = await manager.GetValidTwoFactorProvidersAsync(user);
    bool enabled = providers.Any(_ => _ == "Email" || _ == "Phone");
    return enabled;
  }

  public async Task<string> GenerateAsync(string purpose, UserManager<AppUser> manager, AppUser user)
  {
    var token = await manager.GenerateTwoFactorTokenAsync(user, purpose);

    var emailBody = GenerateEmailBody(token, emailService.GetLogoAsString());

    await emailService.SendEmailAsync(user.Email!, "Código de verificación de dos factores", emailBody, "Obra Social Provincia", true);

    return token;
  }

  public async Task<bool> ValidateAsync(string purpose, string token, UserManager<AppUser> manager, AppUser user)
  {
    var userToken = await userManager.VerifyTwoFactorTokenAsync(user, purpose, token);
    return userToken;
  }

  private static string GenerateEmailBody(string token, string image)
  {
    // HTML personalizado para el correo electrónico
    var htmlBody = $@"
            <html>
                <body style='background-color: #ffffe0;'>
                    <div style='text-align: center;'>
                        <img src='cid:logo.png' alt='{TitleApp}' style='max-width: 300px;' />
                        <h1 style='font-size: 24px; color: #333;'>{TitleApp}</h1>
                        <p style='font-size: 18px;'>Su código de verificación de dos factores es:</p>
                        <div style='background-color: #ffffcc; padding: 10px; border-radius: 5px; font-size: 24px;'>
                            {token}
                        </div>
                    </div>
                </body>
            </html>
        ";

    return htmlBody;
  }
}
