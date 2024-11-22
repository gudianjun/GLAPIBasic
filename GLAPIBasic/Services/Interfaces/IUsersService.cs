using GLAPIBasic.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GLAPIBasic.Services.Interfaces
{
    public interface IUsersService
    {
       
        Task<ActionResult<RegisterResponse>> RegisterAsync(RegisterRequest request); 
        Task<ActionResult<UpdateUserInfoResponse>> UpdateUserInfoAsync(UpdateUserInfoRequest request);
        Task<ActionResult<ChangePasswordResponse>> ChangePasswordAsync(ChangePasswordRequest request);

         

        Task<ActionResult<SendResetPasswordCodeResponse>> SendResetPasswordCodeAsync(SendResetPasswordCodeRequest request);
        Task<ActionResult<CodeResetPasswordResponse>> CodeResetPasswordAsync(CodeResetPasswordRequest request);

        Task<ActionResult<SendCodeResponse>> SendCodeAsync([FromBody] SendCodeRequest request);

        Task<bool> CheckMailExist(string? mail);
    }
}
