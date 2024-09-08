namespace TecnoCredito.Models.DTOs;

public class LoginDTO
{
  public string User { get; set; } = null!;
  public string Password { get; set; } = null!;
  public bool? Machine { get; set; }
}
