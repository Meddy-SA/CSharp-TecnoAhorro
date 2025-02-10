using TecnoCredito.Models.DTOs;

namespace TecnoCredito.Services.Interfaces;

public interface ISysMenu
{
    Task<ResponseDTO<string>> GetMenuForRole(List<int> rolesToFilter);
    Task<bool> AddCategoryAsync(string name, List<int> roles);
}
