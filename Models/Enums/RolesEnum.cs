using System.ComponentModel.DataAnnotations;

namespace TecnoCredito.Models.Enums;

public enum RolesEnum
{
  [Display(Name = "Usuario")]
  Usuario,
  [Display(Name = "Vendedor")]
  Vendedor,
  [Display(Name = "Administrador")]
  Admin,
  [Display(Name = "Super Administrador")]
  SuperAdmin,
}
