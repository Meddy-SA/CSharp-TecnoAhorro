using AutoMapper;
using TecnoCredito.Models.DTOs.Products;
using TecnoCredito.Models.Products;

namespace TecnoCredito.Contexts.AutoMapper;

public class CategoryProfile : Profile
{
  public CategoryProfile()
  {
    CreateMap<Category, CategoryDTO>()
      .ForMember(dest =>
          dest.SubCategories, opt =>
            opt.MapFrom(src =>
              src.SubCategories))
      .ReverseMap();
  }
}
