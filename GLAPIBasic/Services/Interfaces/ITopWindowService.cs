using GLAPIBasic.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GLAPIBasic.Services.Interfaces
{
    public interface ITopWindowService
    {
        Task<ActionResult<LoginResponse>> LoginAsync(LoginRequest request);
        Task LogoutAsync();
        Task<ActionResult<LoginResponse>> RefreshAsync();
        Task<ActionResult<RegisterResponse>> RegisterAsync(RegisterRequest request);
        Task<GetUserInfoResponse> GetUserInfoAsync(string userId);
        Task<ActionResult<UpdateUserInfoResponse>> UpdateUserInfoAsync(UpdateUserInfoRequest request);
        Task<ActionResult<ChangePasswordResponse>> ChangePasswordAsync(ChangePasswordRequest request);


        Task<ActionResult<GetFilesResponse>> GetFilesAsync();
        Task<ActionResult<DownloadFileResponse>> DownloadFileAsync(string fileId);
        Task<ActionResult<CreateFileResponse>> CreateFileAsync(CreateFileRequest request);
        Task<ActionResult<DeleteFileResponse>> DeleteFileAsync(string fileId);
        Task<ActionResult<UpdateFileResponse>> UpdateFileAsync(string fileId, UpdateFileRequest request);

        Task<ActionResult<SendResetPasswordCodeResponse>> SendResetPasswordCodeAsync(SendResetPasswordCodeRequest request);
        Task<ActionResult<CodeResetPasswordResponse>> CodeResetPasswordAsync(CodeResetPasswordRequest request);

        Task<ActionResult<SendCodeResponse>> SendCodeAsync([FromBody] SendCodeRequest request);

        Task<bool> CheckMailExist(string? mail);
    }
}
