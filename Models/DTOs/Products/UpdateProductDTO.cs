namespace TecnoCredito.Models.DTOs.Products;

public record UpdateProductDTO
{
  public string? Name { get; init; }
  public string? Description { get; init; }
  public decimal? Price { get; init; }
  public int? StockQuantity { get; init; }
  public int? CategoryId { get; init; }
  public string? Brand { get; init; }
  public string? Model { get; init; }
  public string? SKU { get; init; }
  public EnumDTO? Status { get; init; }
  public string? TechnicalSpecifications { get; init; }
}
