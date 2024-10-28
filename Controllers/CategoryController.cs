using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TecnoCredito.Models.DTOs;
using TecnoCredito.Models.DTOs.Products;
using TecnoCredito.Services.Interfaces;

[AllowAnonymous]
[ApiController]
[Route("[controller]")]
public class CategoryController(ICategoryHandle category) : ControllerBase
{
  private readonly ICategoryHandle category = category;

  [HttpGet("get")]
  public async Task<ActionResult<ResponseDTO<List<CategoryDTO>>>> GetAll()
  {
    var response = await category.GetAllAsync();
    return this.HandleResponse(response);
  }

  [HttpGet("get/{id}")]
  public async Task<ActionResult<ResponseDTO<CategoryDTO>>> GetById(int id)
  {
    var response = await category.GetByIdAsync(id);
    return this.HandleResponse(response);
  }

  [HttpPost("post")]
  public async Task<ActionResult<ResponseDTO<CategoryDTO>>> Post(CategoryDTO dto)
  {
    var response = await category.CreateAsync(dto);
    return this.HandleResponse(response);
  }

  [HttpPut("put/{id}")]
  public async Task<ActionResult<ResponseDTO<CategoryDTO>>> Put(int id, CategoryDTO dto)
  {
    var response = await category.UpdateAsync(id, dto);
    return this.HandleResponse(response);
  }
}
