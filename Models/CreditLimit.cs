using TecnoCredito.Models.Enums;

namespace TecnoCredito.Models;

public class CreditLimit
{
  public int Id { get; set; }
  public string UserId { get; set; } = null!;
  public decimal Limit { get; set; }
  public decimal Available { get; set; }
  public StatusEnum Status { get; set; }
}
