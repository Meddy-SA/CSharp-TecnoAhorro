using System.ComponentModel.DataAnnotations;

namespace TecnoCredito.Models.Enums;

public enum StatusEnum
{
  [Display(Name = "Activo")]
  Active,
  [Display(Name = "Anulado")]
  Inactive,
  [Display(Name = "Suspendido")]
  Suspended
}
