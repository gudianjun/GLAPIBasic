using GLAPIBasic.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GLAPIBasic.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ActionResult<LoginResponse>> LoginAsync(LoginRequest request);
        Task LogoutAsync();
        Task<ActionResult<LoginResponse>> RefreshAsync(); 
    }
}
