namespace TecnoCredito.Models.DTOs;

public class CurrentAccountSummaryDTO
{
  public decimal TotalBalance { get; init; }
  public decimal AvailableCredit { get; init; }
  public EnumDTO Status { get; init; } = null!;
}
