namespace TecnoCredito.Services.Interfaces;

public interface IEmailSender
{
  /// <summary>
  /// Servicio para enviar email.
  /// </summary>
  /// <param name="email">email de destino</param>
  /// <param name="subject">Motivo de mensaje</param>
  /// <param name="message">Mensaje</param>
  /// <param name="senderName">Nombre del que envia</param>
  /// <param name="generateLogo">Si se envia con el logo.</param>
  /// <returns></returns>
  Task SendEmailAsync(string email, string subject, string message, string senderName, bool generateLogo = false);
  byte[] GetLogoAsBytes();
  string GetLogoAsString();
}
