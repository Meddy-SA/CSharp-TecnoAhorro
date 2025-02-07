using TecnoCredito.Models.DTOs.Products;

namespace TecnoCredito.Services.Interfaces;

public interface IPdfService
{
    Task<byte[]> GenerateProductPdfAsync(List<ProductDTO> model);
}
