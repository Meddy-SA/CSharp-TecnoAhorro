namespace TecnoCredito.Models.Products;

public class ProductFeature
{
  public int ProductId { get; set; }
  public virtual Product Product { get; set; } = null!;
  public int FeatureId { get; set; }
  public virtual Feature Feature { get; set; } = null!;
  public string Value { get; set; } = null!;
}
