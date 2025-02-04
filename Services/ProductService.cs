using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TecnoCredito.Contexts;
using TecnoCredito.Models.DTOs;
using TecnoCredito.Models.DTOs.Products;
using TecnoCredito.Models.Products;
using TecnoCredito.Services.Interfaces;

namespace TecnoCredito.Services;

public class ProductService(Context context, IMapper mapper) : IProductHandle
{
    private readonly Context context = context;
    private readonly IMapper mapper = mapper;

    public async Task<ResponseDTO<List<ProductDTO>>> GetAllAsync()
    {
        var response = new ResponseDTO<List<ProductDTO>>();
        try
        {
            var products = await context.Products.Include(p => p.Category).ToListAsync();

            var result = mapper.Map<List<ProductDTO>>(products);

            response.Result = result;
            response.IsSuccess = true;
        }
        catch (Exception ex)
        {
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ResponseDTO<ProductDTO>> GetByIdAsync(int id)
    {
        var response = new ResponseDTO<ProductDTO>();
        try
        {
            var product = await context
                .Products.Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            var result = mapper.Map<ProductDTO>(product);

            response.Result = result;
            response.IsSuccess = true;
        }
        catch (Exception ex)
        {
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ResponseDTO<ProductDTO>> CreateAsync(ProductDTO productCreate)
    {
        var response = new ResponseDTO<ProductDTO>();
        try
        {
            var product = mapper.Map<Product>(productCreate);
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            response.Result = mapper.Map<ProductDTO>(product);
            response.IsSuccess = true;
        }
        catch (Exception ex)
        {
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ResponseDTO<ProductDTO>> UpdateAsync(int id, ProductDTO productUpdate)
    {
        var response = new ResponseDTO<ProductDTO>();
        try
        {
            var product =
                await context.Products.FirstOrDefaultAsync(p => p.Id == id)
                ?? throw new Exception("Product not found");

            context.Update(product);
            await context.SaveChangesAsync();

            response.Result = mapper.Map<ProductDTO>(product);
            response.IsSuccess = true;
        }
        catch (Exception ex)
        {
            response.Error = ex.Message;
        }

        return response;
    }
}
