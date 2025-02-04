using TecnoCredito.Models.Products;

namespace TecnoCredito.Models;

public class CurrentAccountProduct
{
  public int CurrentAccountId { get; set; }
  public int ProductId { get; set; }
  public virtual CurrentAccount CurrentAccount { get; set; } = null!;
  public virtual Product Product { get; set; } = null!;
}
