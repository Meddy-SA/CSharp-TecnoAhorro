using TecnoCredito.Models.Enums;

namespace TecnoCredito.Models.Products;

public class Product
{
  public int Id { get; set; }
  public string Name { get; set; } = null!;
  public string Description { get; set; } = null!;
  public decimal Price { get; set; }
  public int StockQuantity { get; set; }
  public int CategoryId { get; set; }
  public virtual Category Category { get; set; } = null!;
  public string Brand { get; set; } = null!;
  public string Model { get; set; } = null!;
  public string SKU { get; set; } = null!;
  public StatusEnum Status { get; set; } = StatusEnum.Active;
  public string TechnicalSpecifications { get; set; } = null!;
  public virtual ICollection<ProductImage> ProductImages { get; set; } = [];
  public virtual ICollection<ContractProduct> ContractProducts { get; set; } = [];
  public virtual ICollection<ProductFeature> ProductFeatures { get; set; } = [];
}
