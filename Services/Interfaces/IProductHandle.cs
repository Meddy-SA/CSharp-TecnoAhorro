using TecnoCredito.Models.DTOs;
using TecnoCredito.Models.DTOs.Products;

namespace TecnoCredito.Services.Interfaces;

public interface IProductHandle
{
  Task<ResponseDTO<List<ProductDTO>>> GetAllAsync();
  Task<ResponseDTO<ProductDTO>> GetByIdAsync(int id);
  Task<ResponseDTO<ProductDTO>> CreateAsync(ProductDTO productCreate);
  Task<ResponseDTO<ProductDTO>> UpdateAsync(int id, ProductDTO productUpdate);
}
