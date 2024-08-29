namespace TecnoCredito.Models.Products;

public class Feature
{
  public int Id { get; set; }
  public string Name { get; set; } = null!;
  public string Description { get; set; } = null!;
  public virtual ICollection<ProductFeature> ProductFeatures { get; set; } = [];
}
