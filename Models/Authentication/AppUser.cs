using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TecnoCredito.Models.Authentication;

public class AppUser : IdentityUser
{
  [Required]
  [Display(Name = "Nombre")]
  [StringLength(15)]
  public string FirstName { get; set; } = null!;

  [PersonalData]
  [Display(Name = "Segundo Nombre")]
  [StringLength(15)]
  public string? MiddleName { get; set; }

  [PersonalData]
  [Required]
  [Display(Name = "Apellido")]
  [StringLength(15)]
  public string LastName { get; set; } = null!;

  [PersonalData]
  [Display(Name = "Segundo Apellido")]
  [StringLength(15)]
  public string? SecondSurname { get; set; }

  [PersonalData]
  [Display(Name = "Foto")]
  public byte[]? ProfilePicture { get; set; }

  [Display(Name = "Claims")]
  public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; } = null!;

  [Display(Name = "Nombre Completo")]
  public string FullName() => $"{FirstName} {LastName}";

  [Display(Name = "Foto en Base64")]
  public string? ProfilePictureBase64 => ProfilePicture != null ? Convert.ToBase64String(ProfilePicture) : null;

  public virtual ICollection<CustomerContract> CustomerContracts { get; set; } = [];
  public virtual ICollection<Payment> Payments { get; set; } = [];
  public virtual ICollection<Installment> Installments { get; set; } = [];
}
