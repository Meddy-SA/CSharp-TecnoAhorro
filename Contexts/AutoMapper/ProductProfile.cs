using AutoMapper;
using TecnoCredito.Models.DTOs.Products;
using TecnoCredito.Models.Products;

namespace TecnoCredito.Contexts.AutoMapper;

public class ProductProfile : Profile
{
  public ProductProfile()
  {
    CreateMap<Product, ProductDTO>().ReverseMap();
  }
}
