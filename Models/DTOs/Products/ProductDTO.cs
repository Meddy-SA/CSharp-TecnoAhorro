
namespace TecnoCredito.Models.DTOs.Products;

public record ProductDTO
{
  public int Id { get; init; }
  public string Name { get; init; } = null!;
  public string Description { get; init; } = null!;
  public decimal Price { get; init; }
  public int StockQuantity { get; init; }
  public int CategoryId { get; init; }
  public string Brand { get; init; } = null!;
  public string Model { get; init; } = null!;
  public string SKU { get; init; } = null!;
  public EnumDTO Status { get; init; } = null!;
  public string TechnicalSpecifications { get; init; } = null!;
  public ICollection<ProductImageDTO> ProductImages { get; init; } = [];
  public ICollection<ProductFeatureDTO> ProductFeatures { get; init; } = [];
}
