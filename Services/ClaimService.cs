using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TecnoCredito.Models.Authentication;
using TecnoCredito.Models.DTOs;
using TecnoCredito.Services.Interfaces;

namespace TecnoCredito.Services;

public class ClaimService(IConfiguration configuration) : IClaim
{
  private JwtDTO Jwt { get; set; } = new();
  private readonly IConfiguration configuration = configuration;


  public string GenerateToken(AppUser user, IList<string>? roles)
  {
    configuration.GetSection("JWT").Bind(Jwt);
    var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Jwt!.Key));
    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
    // Crear una lista de Claims para agregar al token
    var claims = new List<Claim>
    {
      new(ClaimTypes.Name, user.FullName()),
      new(ClaimTypes.NameIdentifier, user.Id),
      new(ClaimTypes.GivenName, user.FirstName),
      new(ClaimTypes.Surname, user.LastName),
      new(ClaimTypes.Email, user.Email!.ToLower()),
      new(ClaimTypes.UserData, user.UserName!),
      new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    };

    // Agregar roles como claims si es necesario
    if (roles != null)
    {
      claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
    }


    var securityToken = new JwtSecurityToken(
        issuer: Jwt.Issuer,
        audience: Jwt.Audience,
        claims: claims,
        expires: DateTime.UtcNow.AddDays(8),
        signingCredentials: signIn);

    var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
    return token;
  }
}
