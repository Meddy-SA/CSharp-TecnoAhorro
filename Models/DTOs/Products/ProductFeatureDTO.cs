namespace TecnoCredito.Models.DTOs.Products;

public record ProductFeatureDTO
{
  public int ProductId { get; init; }
  public int FeatureId { get; init; }
  public string Value { get; init; } = null!;
}
