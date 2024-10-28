using TecnoCredito.Models.Authentication;

namespace TecnoCredito.Services.Interfaces;

public interface IClaim
{
  string GenerateToken(AppUser user, IList<string>? roles);
}
