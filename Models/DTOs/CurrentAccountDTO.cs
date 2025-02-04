namespace TecnoCredito.Models.DTOs;

public class CurrentAccountDTO
{
  public int Id { get; set; }
  public DateTime Date { get; set; }
  public EnumDTO Type { get; set; } = null!;
  public decimal Amount { get; set; }
  public decimal Balance { get; set; }
  public string? Description { get; set; }
  public EnumDTO Status { get; set; } = null!;
  public ICollection<CurrentAccountProductDTO> Products { get; init; } = [];
}
