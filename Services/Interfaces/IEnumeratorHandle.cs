using TecnoCredito.Models.DTOs;

namespace TecnoCredito.Services.Interfaces;

public interface IEnumeratorHandle
{
  ResponseDTO<List<EnumDTO>> GetSexo();
  ResponseDTO<List<EnumDTO>> GetRoles();
  ResponseDTO<List<EnumDTO>> GetStatus();
}
