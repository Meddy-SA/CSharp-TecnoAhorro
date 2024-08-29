using TecnoCredito.Models.Enums;

namespace TecnoCredito.Models.Money;

public class PaymentMethod
{
  public int Id { get; set; }
  public string Name { get; set; } = null!;
  public string Description { get; set; } = null!;
  public StatusEnum Status { get; set; } = StatusEnum.Active;

  public virtual ICollection<Payment> Payments { get; set; } = [];
}
