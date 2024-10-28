using Microsoft.AspNetCore.Identity;

namespace TecnoCredito.Models.DTOs;

public class CustomErrorDTO
{
  public string Code { get; set; }
  public string Description { get; set; }

  public CustomErrorDTO(string code, string description)
  {
    Code = code;
    Description = description;
  }

  public CustomErrorDTO(IdentityError identityError)
  {
    Description = identityError.Description;
    Code = identityError.Code;
  }
}
