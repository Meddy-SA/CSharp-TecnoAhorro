using System.ComponentModel.DataAnnotations;

namespace TecnoCredito.Models.Enums;

public enum AccountTransactionTypeEnum
{
  [Display(Name = "Crédito")]
  Credit,
  [Display(Name = "Pago")]
  Payment,
  [Display(Name = "Tasa")]
  Tax,
  [Display(Name = "Interés")]
  Interest
}
