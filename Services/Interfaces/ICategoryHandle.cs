using TecnoCredito.Models.DTOs;
using TecnoCredito.Models.DTOs.Products;

namespace TecnoCredito.Services.Interfaces;

public interface ICategoryHandle
{
  Task<ResponseDTO<List<CategoryDTO>>> GetAllAsync();
  Task<ResponseDTO<CategoryDTO>> GetByIdAsync(int id);
  Task<ResponseDTO<CategoryDTO>> CreateAsync(CategoryDTO categoryCreate);
  Task<ResponseDTO<CategoryDTO>> UpdateAsync(int id, CategoryDTO categoryUpdate);
}
