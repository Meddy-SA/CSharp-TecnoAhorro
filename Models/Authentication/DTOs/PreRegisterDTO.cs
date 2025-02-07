namespace TecnoCredito.Models.Authentication.DTOs;

public class PreRegisterDTO
{
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Country { get; set; } = "ar";
    public string Password { get; set; } = null!;
    public bool TermsAccepted { get; set; }
    public bool EmailUpdated { get; set; }
}
