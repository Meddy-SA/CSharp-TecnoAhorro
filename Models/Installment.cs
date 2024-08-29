using TecnoCredito.Models.Enums;

namespace TecnoCredito.Models;

public class Installment
{
  public int Id { get; set; }
  public int CustomerContractId { get; set; }
  public virtual CustomerContract Contract { get; set; } = null!;
  public decimal Amount { get; set; }
  public DateTime DueDate { get; set; }
  public StatusEnum Status { get; set; } = StatusEnum.Active;
  public int InstallmentNumber { get; set; }
  public decimal? Interest { get; set; }
}
