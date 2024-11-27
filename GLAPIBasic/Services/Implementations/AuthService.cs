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
using Org.BouncyCastle.Asn1.Ocsp;
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

        public  ApiResponse<LoginResponse> getLoginResponse(UserInfo user, string audience)
        {
            string session = Guid.NewGuid().ToString();
            string accessToken = StringHelper.CreateToken(session, user.UserId.ToString()
                , user.Username
                , _configuration["Jwt:Key"]!
                , _configuration["Jwt:Issuer"] ?? "issuer"
                , audience
                , TokenType.AccessToken
                , DateTime.Now.AddMinutes(_apiConfig.AccessTokenExpiresTime));
            string refreshToken = StringHelper.CreateToken(session, user.UserId.ToString()
               , user.Username
               , _configuration["Jwt:Key"]!
               , _configuration["Jwt:Issuer"] ?? "issuer"
               , audience
               , TokenType.RefreshToken
               , DateTime.Now.AddDays(30));

            // 保存用户Token到内存中，用来登录校验
            _authRepository.SaveLoginInfo(user.UserId, audience, session);
            var response = new ApiResponse<LoginResponse>(new LoginResponse()
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                userInfo = new LoginUserInfo()
                {
                    UserId = user.UserId,
                    Username = user.Username!,
                    AvatarThumbnail = user.AvatarThumbnail??"",
                    FirstName = user.FirstName,
                    LastName = user.LastName
                }
            });
            return response;
        }
        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            UserInfo? user = null;
            user = await _usersRepository.GetUserInfoForUserNameAsync(request.Username);
            if (user == null)
            {
                var response = new ApiResponse<LoginResponse>(HttpStatusCode.NotFound, "Incorrect username or password.", null);
                return response ;
            }
            else
            {
                // 密码验证需要加密后验证
                // 检查密码是否正确
                if (!StringHelper.VerifyPassword(request.Password, user!.Password ?? ""))
                {
                    var response = new ApiResponse<LoginResponse>(HttpStatusCode.NotFound, "Incorrect username or password.", null);
                    return response ;
                }
                else
                {
                    return getLoginResponse(user, request.AudienceName);
                }
            }
        }
        public async Task<ApiResponse<LoginResponse>> RefreshAsync()
        {
            var tokenInfo = HttpContextHelper.GetTokenInfo();
            var user = await _usersRepository.GetUserInfoForUserNameAsync(tokenInfo.UserName);
            if (user == null)
            {
                var response = new ApiResponse<LoginResponse>(HttpStatusCode.NotFound, "Incorrect username or password.", null);
                return response;
            }

            return getLoginResponse(user, tokenInfo.Audience); 
        }
        public async Task LogoutAsync()
        {
            var tokenInfo = HttpContextHelper.GetTokenInfo();
            _authRepository.SaveLoginInfo(tokenInfo.UserId, tokenInfo.Audience, "");
            await Task.CompletedTask;
        }
    }
}
