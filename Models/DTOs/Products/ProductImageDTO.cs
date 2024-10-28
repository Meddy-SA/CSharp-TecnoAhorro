namespace TecnoCredito.Models.DTOs.Products;

public record ProductImageDTO
{
  public int Id { get; init; }
  public string Url { get; init; } = null!;
  public int ProductId { get; init; }
}
