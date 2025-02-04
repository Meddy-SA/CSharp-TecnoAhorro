namespace TecnoCredito.Models.DTOs;

public class CreateCurrentAccountDTO
{
  public EnumDTO Type { get; init; } = null!;
  public decimal Amount { get; init; }
  public string? Description { get; init; }
  public List<int> ProductIds { get; init; } = [];
}
