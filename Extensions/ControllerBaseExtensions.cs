using Microsoft.AspNetCore.Mvc;
using TecnoCredito.Models.DTOs;

namespace TecnoCredito.Extensions;

public static class ControllerBaseExtensions
{
  public static ActionResult<ResponseDTO<T>> HandleResponse<T>(this ControllerBase controller, ResponseDTO<T> response, int successStatusCode = StatusCodes.Status200OK)
  {
    if (response.Success)
    {
      return controller.StatusCode(successStatusCode, response);
    }
    else if (response.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
    {
      return controller.NotFound(response);
    }
    else
    {
      return controller.BadRequest(response);
    }
  }
}
