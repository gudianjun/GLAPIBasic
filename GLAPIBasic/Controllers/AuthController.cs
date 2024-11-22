using GLAPIBasic.DTOs;
using GLAPIBasic.Services.Implementations;
using GLAPIBasic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GLAPIBasic.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private readonly IMemoryCache _memoryCache;
        public AuthController(IConfiguration configuration, IAuthService authService
            , ILogger<AuthController> logger, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            return response;
        }
        /// <summary>
        /// Logout
        /// </summary>
        /// <returns></returns>
        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<string>> Logout()
        {
            await _authService.LogoutAsync();
            return (new ApiResponse<string>("Logout Successful")).Result();
        }
        /// <summary>
        /// Refresh token
        /// 当访问token过期时，使用refresh token来获取新的token
        /// </summary>
        /// <returns></returns>
        [HttpPost("refresh")]
        [Authorize]
        public async Task<ActionResult<LoginResponse>> Refresh()
        {
            var response = await _authService.RefreshAsync();
            return response;
        }
    }
}
