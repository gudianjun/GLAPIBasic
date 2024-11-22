using AutoMapper;
using GLAPIBasic.Configurations;
using GLAPIBasic.DTOs;
using GLAPIBasic.Enums;
using GLAPIBasic.Models;
using GLAPIBasic.Repositories.Implementations;
using GLAPIBasic.Repositories.Interfaces;
using GLAPIBasic.Services.Interfaces;
using GLAPIBasic.Utilities;
using GLAPIBasic.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MimeKit;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;


namespace GLAPIBasic.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly APIConfig _apiConfig;
        private readonly IMapper _mapper;
        public AuthService(IConfiguration configuration
            , IAuthRepository authRepository
            , IUsersRepository usersRepository
            , ILogger<AuthService> logger, IMemoryCache memoryCache
            , IHttpContextAccessor httpContextAccessor
            , IOptionsMonitor<APIConfig> apiConfig
            , IMapper mapper)
        {
            _configuration = configuration;
            _authRepository = authRepository;
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
            _logger = logger;
            _usersRepository = usersRepository;
            _configuration = configuration;
            _apiConfig = apiConfig.CurrentValue;
            _mapper = mapper;
        }


        public async Task<ActionResult<LoginResponse>> LoginAsync(LoginRequest request)
        {
            User? user = null;

            user = await _usersRepository.GetUserInfoForUserNameAsync(request.Username);

            if (user == null)
            {
                var response = new ApiResponse<LoginResponse>(HttpStatusCode.NotFound, "Incorrect username or password.", null);
                return response.Result();
            }
            else
            {
                // 密码验证需要加密后验证
                // 检查密码是否正确
                if (!StringHelper.VerifyPassword(request.Password, user!.Password ?? ""))
                {
                    var response = new ApiResponse<LoginResponse>(HttpStatusCode.NotFound, "Incorrect username or password.", null);
                    return response.Result();
                }
                else
                {
                    string session = Guid.NewGuid().ToString();
                    string accessToken = StringHelper.CreateToken(session, user.UserId.ToString()
                        , request.Username
                        , _configuration["Jwt:Key"]!
                        , _configuration["Jwt:Issuer"] ?? "issuer"
                        , request.AudienceName
                        , TokenType.AccessToken
                        , DateTime.Now.AddMinutes(_apiConfig.AccessTokenExpiresTime));
                    string refreshToken = StringHelper.CreateToken(session, user.UserId.ToString()
                       , request.Username
                       , _configuration["Jwt:Key"]!
                       , _configuration["Jwt:Issuer"] ?? "issuer"
                       , request.AudienceName
                       , TokenType.RefreshToken
                       , DateTime.Now.AddDays(30));

                    // 保存用户Token到内存中，用来登录校验
                    await _authRepository.SaveLoginInfoAsync((int)user.UserId, request.AudienceName, session);
                    var response = new ApiResponse<LoginResponse>(new LoginResponse()
                    {
                        Token = accessToken,
                        RefreshToken = refreshToken,
                        userInfo = new UserInfo()
                        {
                            UserId = user.UserId,
                            Name = user.Username!
                        }
                    });
                    return response.Result();
                }
            }
        }
        public async Task<ActionResult<LoginResponse>> RefreshAsync()
        {
            var claimsIdentity = _httpContextAccessor?.HttpContext?.User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                string session = Guid.NewGuid().ToString();
                var userId = claimsIdentity.FindFirst(KeyName.USER_ID)?.Value;
                var userName = claimsIdentity.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                var AudienceName = claimsIdentity.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Aud)?.Value;

                string accessToken = StringHelper.CreateToken(session, userId!
                    , userName!
                    , _configuration["Jwt:Key"]!
                    , _configuration["Jwt:Issuer"] ?? "issuer"
                    , AudienceName!
                    , TokenType.AccessToken
                    , DateTime.Now.AddMinutes(15));
                string refreshToken = StringHelper.CreateToken(session, userId!
                   , userName!
                   , _configuration["Jwt:Key"]!
                   , _configuration["Jwt:Issuer"] ?? "issuer"
                   , AudienceName!
                   , TokenType.RefreshToken
                   , DateTime.Now.AddDays(30));

                // 保存用户Token到内存中，用来登录校验
                await _authRepository.SaveLoginInfoAsync((int)int.Parse(userId!), AudienceName!,
                    session);
                var response = new ApiResponse<LoginResponse>(new LoginResponse()
                {
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    userInfo = null
                });
                return response.Result();
            }
            return (new ApiResponse<LoginResponse>(HttpStatusCode.NotFound, "Refresh token failed, need to log in again", null)).Result();
        }
        public async Task LogoutAsync()
        {
            var claimsIdentity = _httpContextAccessor?.HttpContext?.User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                var userId = claimsIdentity.FindFirst(KeyName.USER_ID)?.Value;
                var aud = claimsIdentity.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Aud)?.Value;
                if (userId != null && aud != null)
                {
                    await _authRepository.SaveLoginInfoAsync(int.Parse(userId), aud, "");
                }
            }
            throw new NotImplementedException();
        } 
    }
}
