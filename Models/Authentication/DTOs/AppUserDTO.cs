using Microsoft.AspNetCore.Identity;

namespace TecnoCredito.Models.Authentication.DTOs;

public class AppUserDTO
{
  public string UserName { get; set; } = null!;
  public string Email { get; set; } = null!;
  public string FirstName { get; set; } = null!;
  public string LastName { get; set; } = null!;
  public string Password { get; set; } = null!;
  public string? FullName { get; set; }
  public string? UserId { get; set; }
  public string? Token { get; set; }
  public string? Avatar { get; set; }
  public string? Menu { get; set; }
  public List<IdentityError>? Messages { get; set; } = default!;
}
