using System.ComponentModel.DataAnnotations;
using TecnoCredito.Middlewares;

namespace TecnoCredito.Models.Enums;

public enum StatusEnum
{
  [Display(Name = "Activo")]
  [Severity("success")]
  Active,
  [Display(Name = "Anulado")]
  [Severity("danger")]
  Inactive,
  [Display(Name = "Suspendido")]
  [Severity("warning")]
  Suspended
}
