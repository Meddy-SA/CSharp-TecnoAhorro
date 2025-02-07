using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TecnoCredito.Models.Authentication.DTOs;
using TecnoCredito.Models.DTOs;
using TecnoCredito.Services.Interfaces;

namespace TecnoCredito.Controllers;

[AllowAnonymous]
[ApiController]
[Route("[controller]")]
public class AuthenticationController(IUserHandle userHandle) : ControllerBase
{
    private readonly IUserHandle userHandle = userHandle;

    [HttpGet("re-send/{email}")]
    public async Task<ActionResult<ResponseDTO<string>>> ReSend(string email)
    {
        var response = await userHandle.ReSendEmailValidate(email);
        return this.HandleResponse(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<ResponseDTO<AppUserDTO>>> Login(LoginDTO loginDTO)
    {
        var response = await userHandle.Login(loginDTO);
        return this.HandleResponse(response);
    }

    [HttpPost("register")]
    public async Task<ActionResult<ResponseDTO<PreRegisterDTO>>> PreRegister(
        PreRegisterDTO preRegisterDTO
    )
    {
        var response = await userHandle.PreRegister(preRegisterDTO);
        return this.HandleResponse(response);
    }

    [HttpPost("create-account")]
    public async Task<ActionResult<ResponseDTO<AppUserDTO>>> CreateAccount(AppUserDTO userDTO)
    {
        var responseDTO = await userHandle.CreateUser(userDTO);
        return this.HandleResponse(responseDTO);
    }

    [HttpPut("profile/update")]
    public async Task<ActionResult<ResponseDTO<AppUserDTO>>> ProfileUpdate(AppUserDTO userDTO)
    {
        var responseDTO = await userHandle.CreateUser(userDTO);
        return Ok(new { responseDTO });
    }
}
