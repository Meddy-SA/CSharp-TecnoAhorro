using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TecnoCredito.Models.DTOs;
using TecnoCredito.Services.Interfaces;

[AllowAnonymous]
[ApiController]
[Route("[controller]")]
public class EnumeratorController(IEnumeratorHandle enumerator) : ControllerBase
{
    private readonly IEnumeratorHandle enumerator = enumerator;

    [HttpGet("get-sexos")]
    public ActionResult<ResponseDTO<List<EnumDTO>>> GetSexo()
    {
        var response = enumerator.GetSexo();
        return this.HandleResponse(response);
    }

    [HttpGet("get-roles")]
    public ActionResult<ResponseDTO<List<EnumDTO>>> GetRoles()
    {
        var response = enumerator.GetRoles();
        return this.HandleResponse(response);
    }

    [HttpGet("get-status")]
    public ActionResult<ResponseDTO<List<EnumDTO>>> GetStatus()
    {
        var response = enumerator.GetStatus();
        return this.HandleResponse(response);
    }
}
