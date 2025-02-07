namespace TecnoCredito.Models.DTOs;

public class EmailDTO
{
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public bool EnableSSL { get; set; }
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Sender { get; set; } = null!;
}
