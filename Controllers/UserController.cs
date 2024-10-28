using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TecnoCredito.Models.Authentication.DTOs;
using TecnoCredito.Models.DTOs;
using TecnoCredito.Services.Interfaces;

namespace TecnoCredito.Controllers;

[AllowAnonymous]
[ApiController]
[Route("[controller]")]
public class UserController(IUserHandle userHandle) : ControllerBase
{
  private readonly IUserHandle userHandle = userHandle;

  [HttpGet("ReSendCode/{email}")]
  public async Task<ActionResult<ResponseDTO<string>>> ReSend(string email)
  {
    var response = await userHandle.ReSendEmailValidate(email);
    return Ok(new { response });
  }

  [HttpPost("Login")]
  public async Task<ActionResult<ResponseDTO<AppUserDTO>>> Login(LoginDTO loginDTO)
  {
    var response = await userHandle.Login(loginDTO);
    return Ok(new { response });
  }

  [HttpPost]
  public async Task<ActionResult<ResponseDTO<AppUserDTO>>> CreateAccount(AppUserDTO userDTO)
  {
    var responseDTO = await userHandle.CreateUser(userDTO);
    return Ok(new { responseDTO });
  }


  // Metodos para llenar datos de prueba.

  [HttpGet("CreateRoles")]
  public async Task<ActionResult> CreateRoles()
  {
    await userHandle.EnsureRolesAsync();
    return Ok();
  }

  [HttpGet("CreateTestUser")]
  public async Task<ActionResult> CreateUser()
  {
    await userHandle.EnsureTestUserAsync();
    return Ok();
  }

  [HttpGet("CreateMenu")]
  public async Task<ActionResult> MenuCreate()
  {
    await userHandle.EnsureTestMenuAsync();
    return Ok();
  }
}
