using TecnoCredito.Models.DTOs;
using TecnoCredito.Models.Enums;

namespace TecnoCredito.Services.Interfaces;

public interface ISysMenu
{
  Task<ResponseDTO<string>> GetMenuForRole(List<RolesEnum> rolesToFilter);
  Task<bool> AddCategoryAsync(string name, List<RolesEnum> roles);
}
