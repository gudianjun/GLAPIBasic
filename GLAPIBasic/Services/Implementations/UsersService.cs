using AutoMapper;
using GLAPIBasic.Configurations;
using GLAPIBasic.DTOs;
using GLAPIBasic.Enums;
using GLAPIBasic.Models;
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
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UsersService> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly APIConfig _apiConfig;
        private readonly IMapper _mapper;
        public UsersService(IConfiguration configuration
            , IUsersRepository usersRepository
            , ILogger<UsersService> logger
            , IMemoryCache memoryCache
            , IHttpContextAccessor httpContextAccessor
            , IOptionsMonitor<APIConfig> apiConfig
            , IMapper mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
            _logger = logger;
            _usersRepository = usersRepository;
            _configuration = configuration;
            _apiConfig = apiConfig.CurrentValue;
            _mapper = mapper;
        } 
        public async Task<ApiResponse<ChangePasswordResponse>> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var tokenInfo = HttpContextHelper.GetTokenInfo();
            UserInfo? user = null;
            user = await _usersRepository.GetUserByIdAsync(tokenInfo.UserId);
            if (user != null)
            {
                if (!StringHelper.VerifyPassword(request.OldPassword, user.Password ?? ""))
                {
                    return new ApiResponse<ChangePasswordResponse>(HttpStatusCode.NotFound, "Old password is incorrect", null);
                }
                else
                {
                    user.Password = StringHelper.HashPassword(request.NewPassword);
                    int nCount = await _usersRepository.UpdateUserAsync(user);
                    if (nCount > 0)
                    {
                        return new ApiResponse<ChangePasswordResponse>(null);
                    }
                    else
                    {
                        return new ApiResponse<ChangePasswordResponse>(HttpStatusCode.NotFound, "Update Error!", null);
                    }
                }
            }
            else
            {
                throw new NotImplementedException("Not logged in or verification information is lost");
            } 
        }
         
        public async Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            string code = _usersRepository.LoadResetPasswordCode("SendCode_" + request.UserName);
            if (code == request.ResetCode)
            {// 生成用户数据，并保存到数据库 
                UserInfo user = _mapper.Map<UserInfo>(request);
                await _usersRepository.NewUserAsync(user);
                return new ApiResponse<RegisterResponse>(HttpStatusCode.OK, "Successful registration", null);
            }
            return new ApiResponse<RegisterResponse>(HttpStatusCode.NotFound, "Incorrect verification code", null);
        }
        public async Task<ApiResponse<SendCodeResponse>> SendCodeAsync([FromBody] SendCodeRequest request)
        {
            string toEmail = request.Email;
            // 生成随机5位数字验证码
            Random random = new Random();
            int code = random.Next(10000, 99999);
            _usersRepository.SaveResetPasswordCode("SendCode_" + toEmail, code.ToString());

            string message = $"Registration verification code is: {code}";
            string subject = "Registration verification code";
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_apiConfig.MailYourName, _apiConfig.SmtpUser));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = message };
            await StringHelper.SendEmailAsync(toEmail, subject, _apiConfig, emailMessage);
            return new ApiResponse<SendCodeResponse>(HttpStatusCode.OK,
            "The verification code has been sent to the specified email address", null);
        }
        public async Task<ApiResponse<UpdateUserInfoResponse>> UpdateUserInfoAsync(UpdateUserInfoRequest request)
        {
            var tokenInfo = HttpContextHelper.GetTokenInfo();
            UserInfo? user = await _usersRepository.GetUserByIdAsync(tokenInfo.UserId); 
            if(!string.IsNullOrEmpty(request.FirstName))
            {
                user!.FirstName = request.FirstName;
            }
            if (!string.IsNullOrEmpty(request.LastName))
            {
                user!.LastName = request.LastName;
            }
            if (!string.IsNullOrEmpty(request.AvatarThumbnail))
            {
                user!.AvatarThumbnail = request.AvatarThumbnail;
            }
            await _usersRepository.UpdateUserAsync(user!);
            return new ApiResponse<UpdateUserInfoResponse>(new UpdateUserInfoResponse() {  
                FirstName = user!.FirstName,
                LastName = user!.LastName,
                AvatarThumbnail = user!.AvatarThumbnail??"",
                Username = user!.Username 
            });
        }
        public async Task<GetUserInfoResponse> GetUserInfoAsync(long userId)
        {
            var userInfo = await _usersRepository.GetUserByIdAsync(userId);

            if (userInfo != null)
            {
                return _mapper.Map<GetUserInfoResponse>(userInfo);
            }
            throw new KeyNotFoundException("User not found");
        }
        public async Task<ApiResponse<SendResetPasswordCodeResponse>> SendResetPasswordCodeAsync(SendResetPasswordCodeRequest request)
        {
            var user = await _usersRepository.GetUserInfoForUserNameAsync(request.UserName);
            if (user != null)
            {
                string toEmail = request.UserName;
                // 生成随机5位数字验证码
                Random random = new Random();
                int code = random.Next(10000, 99999);
                _usersRepository.SaveResetPasswordCode(toEmail, code.ToString());

                string message = $"Your password reset code is: {code}";
                string subject = "Password Reset Code";
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(_apiConfig.MailYourName, _apiConfig.SmtpUser));
                emailMessage.To.Add(new MailboxAddress("", toEmail));
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart("plain") { Text = message };
                await StringHelper.SendEmailAsync(toEmail, subject, _apiConfig, emailMessage);
                return new ApiResponse<SendResetPasswordCodeResponse>(HttpStatusCode.OK,
                "The verification code has been sent to the specified email address", null);
            }
            else
            {
                return new ApiResponse<SendResetPasswordCodeResponse>(HttpStatusCode.NotFound, "Email not found", null);
            }

        }

        public async Task<ApiResponse<CodeResetPasswordResponse>> CodeResetPasswordAsync(CodeResetPasswordRequest request)
        {
            string code = _usersRepository.LoadResetPasswordCode(request.UserName);
            if (code == request.ResetCode)
            {
                var user = await _usersRepository.GetUserInfoForUserNameAsync(request.UserName);
                if (user != null)
                {
                    user.Password = StringHelper.HashPassword(request.NewPassword);
                    await _usersRepository.UpdateUserAsync(user);
                    return new ApiResponse<CodeResetPasswordResponse>(null);
                }
                else
                {
                    return new ApiResponse<CodeResetPasswordResponse>(HttpStatusCode.NotFound, "User not found", null);
                }
            }
            else
            {
                return new ApiResponse<CodeResetPasswordResponse>(HttpStatusCode.NotFound, "Verification code error", null);
            }
        }

        /// <summary>
        /// 检查邮箱是否存在(Username)
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CheckMailExist(string? mail)
        {
            if (string.IsNullOrEmpty(mail))
            {
                return true;
            }
            var has = await _usersRepository.GetUserInfoForUserNameAsync(mail);
            return has== null ? false : true;
        }
    }
}
