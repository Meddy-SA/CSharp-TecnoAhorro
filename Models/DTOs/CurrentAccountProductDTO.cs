namespace TecnoCredito.Models.DTOs;

public record CurrentAccountProductDTO
{
  public int ProductId { get; init; }
  public string ProductName { get; init; } = null!;
  public decimal ProductPrice { get; init; }
}
