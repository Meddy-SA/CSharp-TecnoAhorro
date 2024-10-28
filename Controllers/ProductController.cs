using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TecnoCredito.Models.DTOs;
using TecnoCredito.Models.DTOs.Products;
using TecnoCredito.Services.Interfaces;

[AllowAnonymous]
[ApiController]
[Route("[controller]")]
public class ProductController(IProductHandle product) : ControllerBase
{
  private readonly IProductHandle product = product;

  [HttpGet("get")]
  public async Task<ActionResult<ResponseDTO<List<ProductDTO>>>> GetAll()
  {
    var response = await product.GetAllAsync();
    return this.HandleResponse(response);
  }

  [HttpGet("get/{id}")]
  public async Task<ActionResult<ResponseDTO<ProductDTO>>> GetById(int id)
  {
    var response = await product.GetByIdAsync(id);
    return this.HandleResponse(response);
  }

  [HttpPost("post")]
  public async Task<ActionResult<ResponseDTO<ProductDTO>>> Post(ProductDTO dto)
  {
    var response = await product.CreateAsync(dto);
    return this.HandleResponse(response);
  }

  [HttpPut("put/{id}")]
  public async Task<ActionResult<ResponseDTO<ProductDTO>>> Put(int id, ProductDTO dto)
  {
    var response = await product.UpdateAsync(id, dto);
    return this.HandleResponse(response);
  }
}
