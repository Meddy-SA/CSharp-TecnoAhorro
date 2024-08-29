namespace TecnoCredito.Models.Products;

public class ProductImage
{
  public int Id { get; set; }
  public string Url { get; set; } = null!;
  public byte[] Content { get; set; } = null!;
  public int ProductId { get; set; }
  public virtual Product Product { get; set; } = null!;
}
