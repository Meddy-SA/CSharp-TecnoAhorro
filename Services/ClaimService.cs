using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TecnoCredito.Models.Authentication;
using TecnoCredito.Models.System;
using TecnoCredito.Services.Interfaces;

namespace TecnoCredito.Services;

public class ClaimService(IConfiguration configuration) : IClaim
{
    private JwtSettings Jwt { get; set; } = new();
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

        var tokenLife = Jwt.LifeType switch
        {
            "Day" => DateTime.Now.AddDays(Jwt.TokenLifeTime),
            "Minute" => DateTime.Now.AddMinutes(Jwt.TokenLifeTime),
            "Second" => DateTime.Now.AddSeconds(Jwt.TokenLifeTime),
            _ => DateTime.Now.AddMonths(Jwt.TokenLifeTime),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = tokenLife,
            SigningCredentials = signIn,
            Issuer = Jwt.Issuer,
            Audience = Jwt.Audience,
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
