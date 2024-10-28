namespace TecnoCredito.Models.DTOs;

public class JwtDTO
{
  public string Key { get; set; } = null!;
  public string Issuer { get; set; } = null!;
  public string Audience { get; set; } = null!;
}
