namespace TecnoCredito.Models.Authentication.DTOs;

public class ValidateEmailDTO
{
  public string UserId { get; set; } = null!;
  public string Code { get; set; } = null!;
}
