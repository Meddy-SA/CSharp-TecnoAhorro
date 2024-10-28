using TecnoCredito.Models.DTOs;
using TecnoCredito.Models.Enums;
using TecnoCredito.Services.Interfaces;

namespace TecnoCredito.Services;

public class EnumeratorService : IEnumeratorHandle
{

  public ResponseDTO<List<EnumDTO>> GetSexo()
  {
    var response = new ResponseDTO<List<EnumDTO>> { Success = false };
    try
    {

      response.Result = Enum.GetValues(typeof(SexoEnum))
        .Cast<SexoEnum>().Select(s => new EnumDTO
        {
          Id = (int)s,
          Name = s.GetDisplayName()
        }).ToList();
      response.Success = true;
    }
    catch (Exception ex)
    {
      response.Error = ex.Message;
    }

    return response;
  }

  public ResponseDTO<List<EnumDTO>> GetRoles()
  {
    var response = new ResponseDTO<List<EnumDTO>> { Success = false };
    try
    {
      response.Result = Enum.GetValues(typeof(RolesEnum))
      .Cast<RolesEnum>()
      .Where(role => role != RolesEnum.SuperAdmin)
      .Select(s => new EnumDTO
      {
        Id = (int)s,
        Name = s.GetDisplayName()
      }).ToList();
      response.Success = true;
    }
    catch (Exception ex)
    {
      response.Error = ex.Message;
    }
    return response;
  }

  public ResponseDTO<List<EnumDTO>> GetStatus()
  {
    var response = new ResponseDTO<List<EnumDTO>> { Success = false };
    try
    {
      response.Result = Enum.GetValues(typeof(StatusEnum))
          .Cast<StatusEnum>()
          .Select(s => new EnumDTO
          {
            Id = (int)s,
            Name = s.GetDisplayName(),
            Severity = s.GetSeverity()
          }).ToList();
      response.Success = true;
    }
    catch (Exception ex)
    {
      response.Error = ex.Message;
    }
    return response;
  }
}
