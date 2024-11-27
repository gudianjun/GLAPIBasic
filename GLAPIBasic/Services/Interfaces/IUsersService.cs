using GLAPIBasic.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GLAPIBasic.Services.Interfaces
{
    public interface IUsersService
    {
       
        Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request); 
        Task<ApiResponse<UpdateUserInfoResponse>> UpdateUserInfoAsync(UpdateUserInfoRequest request);

        Task<GetUserInfoResponse> GetUserInfoAsync(long userId);

        Task<ApiResponse<ChangePasswordResponse>> ChangePasswordAsync(ChangePasswordRequest request);

         

        Task<ApiResponse<SendResetPasswordCodeResponse>> SendResetPasswordCodeAsync(SendResetPasswordCodeRequest request);
        Task<ApiResponse<CodeResetPasswordResponse>> CodeResetPasswordAsync(CodeResetPasswordRequest request);

        Task<ApiResponse<SendCodeResponse>> SendCodeAsync([FromBody] SendCodeRequest request);

        Task<bool> CheckMailExist(string? mail);
    }
}
