using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TecnoCredito.Models.Authentication.DTOs;
using TecnoCredito.Models.DTOs;
using TecnoCredito.Services.Interfaces;

namespace TecnoCredito.Controllers;

[AllowAnonymous]
[ApiController]
[Route("[controller]")]
public class AuthenticationController(IAuthenticationHandle authHandle) : ControllerBase
{
    private readonly IAuthenticationHandle authHandle = authHandle;

    [HttpGet("re-send/{email}")]
    public async Task<ActionResult<ResponseDTO<string>>> ReSend(string email)
    {
        var response = await authHandle.ReSendEmailValidate(email);
        return this.HandleResponse(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<ResponseDTO<AppUserDTO>>> Login(LoginDTO loginDTO)
    {
        var response = await authHandle.Login(loginDTO);
        return this.HandleResponse(response);
    }

    [HttpPost("create-account")]
    public async Task<ActionResult<ResponseDTO<AppUserDTO>>> CreateAccount(PreRegisterDTO userDTO)
    {
        var responseDTO = await authHandle.CreateUser(userDTO);
        return this.HandleResponse(responseDTO);
    }

    [HttpPut("profile/update")]
    public ActionResult<ResponseDTO<AppUserDTO>> ProfileUpdate(AppUserDTO userDTO)
    {
        return Ok();
    }
}
