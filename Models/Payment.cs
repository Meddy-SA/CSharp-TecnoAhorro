using TecnoCredito.Models.Authentication;
using TecnoCredito.Models.Enums;
using TecnoCredito.Models.Money;

namespace TecnoCredito.Models;

public class Payment
{
  public int Id { get; set; }
  public string UserId { get; set; } = null!;
  public virtual AppUser User { get; set; } = null!;
  public int ContractId { get; set; }
  public virtual CustomerContract CustomerContract { get; set; } = null!;
  public decimal Amount { get; set; }
  public DateTime PaymentDate { get; set; }
  public int PaymentMethodId { get; set; }
  public virtual PaymentMethod PaymentMethod { get; set; } = null!;
  public StatusEnum Status { get; set; } = StatusEnum.Active;
  public string PaymentProof { get; set; } = null!;
}
