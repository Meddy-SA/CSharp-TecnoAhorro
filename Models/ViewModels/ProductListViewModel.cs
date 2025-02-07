using TecnoCredito.Models.DTOs.Products;

namespace TecnoCredito.Models.ViewModels;

public class ProductListViewModel
{
    public List<ProductDTO>? Products { get; set; }
    public DateTime? GeneratedDate { get; set; }
}
