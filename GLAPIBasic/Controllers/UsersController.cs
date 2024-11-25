using GLAPIBasic.DTOs;
using GLAPIBasic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GLAPIBasic.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {

        private readonly IUsersService _usersService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UsersController> _logger;
        private readonly IMemoryCache _memoryCache;
        public UsersController(IConfiguration configuration, IUsersService usersService
            , ILogger<UsersController> logger, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _usersService = usersService;
            _configuration = configuration;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<RegisterResponse>> Register(RegisterRequest request)
        {
            var response = await _usersService.RegisterAsync(request);
            // 实现用户信息修改逻辑
            return response;
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult<UpdateUserInfoResponse>> UpdateUserInfo([FromBody] UpdateUserInfoRequest request)
        {
            var response = await _usersService.UpdateUserInfoAsync(request);
            // 实现用户信息修改逻辑
            return response;
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("password")]
        public async Task<ActionResult<ChangePasswordResponse>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var resopnse = await _usersService.ChangePasswordAsync(request);
            // 实现修改密码逻辑
            return resopnse;
        }

        /// <summary>
        /// 发送找回密码验证码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("send-reset-password-code")]
        [AllowAnonymous]
        public async Task<ActionResult<SendResetPasswordCodeResponse>> SendResetPasswordCode([FromBody] SendResetPasswordCodeRequest request)
        {
            var response = await _usersService.SendResetPasswordCodeAsync(request);
            return response;
        }

        /// <summary>
        /// 验证码修改密码
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<ActionResult<CodeResetPasswordResponse>> CodeResetPassword([FromBody] CodeResetPasswordRequest request)
        {
            var response = await _usersService.CodeResetPasswordAsync(request);
            return response;
        }

        [HttpPost("send-code")]
        [AllowAnonymous]
        public async Task<ActionResult<SendCodeResponse>> SendCode([FromBody] SendCodeRequest request)
        {
            var response = await _usersService.SendCodeAsync(request);
            return response;
        }
    }
}
