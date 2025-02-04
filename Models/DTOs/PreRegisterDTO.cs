namespace TecnoCredito.Models.DTOs;

public class PreRegisterDTO
{
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Country { get; set; } = "ar";
    public bool TermsAccepted { get; set; }
    public bool EmailUpdated { get; set; }
}
