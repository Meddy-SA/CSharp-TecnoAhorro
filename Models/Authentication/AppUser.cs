using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TecnoCredito.Models.Authentication;

public class AppUser : IdentityUser
{
    [Required]
    [Display(Name = "Nombre")]
    public string FirstName { get; set; } = null!;

    [PersonalData]
    [Display(Name = "Segundo Nombre")]
    public string? MiddleName { get; set; }

    [PersonalData]
    [Required]
    [Display(Name = "Apellido")]
    public string LastName { get; set; } = null!;

    [PersonalData]
    [Display(Name = "Segundo Apellido")]
    public string? SecondSurname { get; set; }

    [PersonalData]
    [Display(Name = "Fecha de Nacimiento")]
    public DateTime? BirthDate { get; set; }

    [PersonalData]
    [Display(Name = "Foto")]
    public byte[]? ProfilePicture { get; set; }

    [Display(Name = "Claims")]
    public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; } = null!;

    [Display(Name = "Nombre Completo")]
    public string FullName() => $"{FirstName} {LastName}";

    [Display(Name = "Foto en Base64")]
    public string? ProfilePictureBase64 =>
        ProfilePicture != null ? Convert.ToBase64String(ProfilePicture) : null;

    public bool IsActive { get; set; }

    public virtual ICollection<CustomerContract> CustomerContracts { get; set; } = [];
    public virtual ICollection<Payment> Payments { get; set; } = [];
    public virtual ICollection<Installment> Installments { get; set; } = [];
    public virtual ICollection<PreRegister> PreRegisters { get; set; } = [];

    public virtual ICollection<AppUserRole> UserRoles { get; set; }

    public AppUser()
    {
        UserRoles = new HashSet<AppUserRole>();
    }

    // Método de ayuda para obtener el nombre completo
    public string ToFullName()
    {
        var parts = new List<string> { FirstName };

        if (!string.IsNullOrWhiteSpace(MiddleName))
            parts.Add(MiddleName);

        parts.Add(LastName);

        if (!string.IsNullOrWhiteSpace(SecondSurname))
            parts.Add(SecondSurname);

        return string.Join(" ", parts);
    }
}
