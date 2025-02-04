using AutoMapper;
using TecnoCredito.Models.Authentication;
using TecnoCredito.Models.DTOs;

namespace TecnoCredito.Contexts.AutoMapper;

public class PreRegisterProfile : Profile
{
    public PreRegisterProfile()
    {
        CreateMap<PreRegister, PreRegisterDTO>().ReverseMap();
    }
}
