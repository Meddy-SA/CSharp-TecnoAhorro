using TecnoCredito.Models.Authentication.DTOs;
using TecnoCredito.Models.DTOs;

namespace TecnoCredito.Services.Interfaces;

public interface IAuthenticationHandle
{
    Task<ResponseDTO<AppUserDTO>> Login(LoginDTO loginDTO);
    Task<ResponseDTO<AppUserDTO>> LoginWith2FA(LoginDTO loginDTO);
    Task<ResponseDTO<string>> ReSendEmailValidate(string email);
    Task<ResponseDTO<string>> ValidateEmail(ValidateEmailDTO validateEmailDTO);
    Task<ResponseDTO<AppUserDTO>> CreateUser(
        PreRegisterDTO preRegisterDto,
        string returnUrl = null!
    );
}
