using TecnoCredito.Models.Enums;

namespace TecnoCredito.Models;

public class CurrentAccount
{
  public int Id { get; set; }
  public string UserId { get; set; } = null!;
  public DateTime Date { get; set; }
  public AccountTransactionTypeEnum Type { get; set; }
  public decimal Amount { get; set; }
  public decimal Balance { get; set; }
  public string? Description { get; set; }
  public StatusEnum Status { get; set; }
  public virtual ICollection<CurrentAccountProduct> CurrentAccountProducts { get; set; } = [];
}
