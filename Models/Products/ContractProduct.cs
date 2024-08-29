namespace TecnoCredito.Models.Products;

public class ContractProduct
{
  public int CustomerContractId { get; set; }
  public virtual CustomerContract CustomerContract { get; set; } = null!;
  public int ProductId { get; set; }
  public virtual Product Product { get; set; } = null!;
  public int Quantity { get; set; }
  public decimal UnitPrice { get; set; }
}
