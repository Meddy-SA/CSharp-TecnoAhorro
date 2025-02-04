namespace TecnoCredito.Models.Authentication;

public class PreRegister
{
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Country { get; set; } = "ar";
    public bool TermsAccepted { get; set; }
    public bool EmailUpdated { get; set; }
    public string? AppUserId { get; set; }
    public virtual AppUser? AppUser { get; set; }
}
