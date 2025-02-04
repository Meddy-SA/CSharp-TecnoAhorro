using System.ComponentModel.DataAnnotations;

namespace TecnoCredito.Models.Enums;

public enum RolesEnum
{
  [Display(Name = "Visitante")]
  Visitante = 0,
  [Display(Name = "Usuario")]
  Usuario = 1,
  [Display(Name = "Vendedor")]
  Vendedor = 2,
  [Display(Name = "Administrador")]
  Admin = 99,
  [Display(Name = "Super Administrador")]
  SuperAdmin = 100,
}
