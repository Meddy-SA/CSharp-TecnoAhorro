using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TecnoCredito.Contexts;
using TecnoCredito.Models.DTOs;
using TecnoCredito.Models.DTOs.Products;
using TecnoCredito.Models.Products;
using TecnoCredito.Services.Interfaces;

namespace TecnoCredito.Services;

public class CategoryService(Context context, IMapper mapper) : ICategoryHandle
{
    private readonly Context context = context;
    private readonly IMapper mapper = mapper;

    public async Task<ResponseDTO<List<CategoryDTO>>> GetAllAsync()
    {
        var response = new ResponseDTO<List<CategoryDTO>>();
        try
        {
            var categories = await context.Categories.ToListAsync();

            var result = mapper.Map<List<CategoryDTO>>(categories);

            response.Result = result;
            response.IsSuccess = true;
        }
        catch (Exception ex)
        {
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ResponseDTO<CategoryDTO>> GetByIdAsync(int id)
    {
        var response = new ResponseDTO<CategoryDTO>();
        try
        {
            var category = await context.Categories.FirstOrDefaultAsync(p => p.Id == id);

            var result = mapper.Map<CategoryDTO>(category);

            response.Result = result;
            response.IsSuccess = true;
        }
        catch (Exception ex)
        {
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ResponseDTO<CategoryDTO>> CreateAsync(CategoryDTO categoryCreate)
    {
        var response = new ResponseDTO<CategoryDTO>();
        try
        {
            var category = mapper.Map<Category>(categoryCreate);
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();

            response.Result = mapper.Map<CategoryDTO>(category);
            response.IsSuccess = true;
        }
        catch (Exception ex)
        {
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ResponseDTO<CategoryDTO>> UpdateAsync(int id, CategoryDTO categoryUpdate)
    {
        var response = new ResponseDTO<CategoryDTO>();
        try
        {
            var existingCategory = await context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if (existingCategory == null)
            {
                response.IsSuccess = false;
                response.Error = "Categoría no encontrada";
                return response;
            }

            if (id != categoryUpdate.Id)
            {
                response.IsSuccess = false;
                response.Error =
                    "El ID en la ruta no coincide con el ID en el cuerpo de la solicitud";
                return response;
            }

            mapper.Map(categoryUpdate, existingCategory);
            context.Entry(existingCategory).State = EntityState.Modified;

            await context.SaveChangesAsync();

            response.Result = mapper.Map<CategoryDTO>(existingCategory);
            response.IsSuccess = true;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await CategoryExistsAsync(id))
            {
                response.IsSuccess = false;
                response.Error = "Categoría no encontrada";
            }
            else
            {
                throw;
            }
        }
        catch (DbUpdateException dbEx)
        {
            response.IsSuccess = false;
            response.Error = $"Error de base de datos: {dbEx.Message}";
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Error = $"Error al actualizar la categoría: {ex.Message}";
        }
        return response;
    }

    private async Task<bool> CategoryExistsAsync(int id)
    {
        return await context.Categories.AnyAsync(e => e.Id == id);
    }
}
