using GLAPIBasic.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GLAPIBasic.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
        Task LogoutAsync();
        Task<ApiResponse<LoginResponse>> RefreshAsync(); 
    }
}
