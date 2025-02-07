namespace TecnoCredito.Services.Interfaces;

public interface IEmailSender
{
    Task SendEmailAsync(
        string email,
        string subject,
        string htmlContent,
        bool generateLogo = false
    );
    Task SendWelcomeEmailAsync(string email, string userName);
    byte[] GetLogoAsBytes();
    string GetLogoAsString();
}
