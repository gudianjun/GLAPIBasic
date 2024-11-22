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
        public async Task<ActionResult<ChangePasswordResponse>> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var claimsIdentity = _httpContextAccessor?.HttpContext?.User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                var userId = claimsIdentity.FindFirst(KeyName.USER_ID)?.Value;
                if (userId != null)
                {
                    User? user = null;
                    if (uint.TryParse(userId, out uint id))
                    {
                        user = await _usersRepository.GetUserByIdAsync(id);
                    }
                    if (user != null)
                    {
                        if (!StringHelper.VerifyPassword(request.OldPassword, user.Password ?? ""))
                        {
                            return new ApiResponse<ChangePasswordResponse>(HttpStatusCode.NotFound, "Old password is incorrect", null).Result();
                        }
                        else
                        {
                            user.Password = StringHelper.HashPassword(request.NewPassword);
                            int ncount = await _usersRepository.UpdateUserAsync(user);
                            if (ncount > 0)
                            {
                                return new ApiResponse<ChangePasswordResponse>(null).Result();
                            }
                            else
                            {
                                return new ApiResponse<ChangePasswordResponse>(HttpStatusCode.NotFound, "Update Error!", null).Result();
                            }
                        }
                    }
                    else
                    {
                        throw new NotImplementedException("Not logged in or verification information is lost");
                    }
                }
                else
                {
                    throw new NotImplementedException("There is no user information in the token");
                }
            }
            throw new NotImplementedException("Not logged in or verification information is lost");
        }


        public async Task<ActionResult<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            string code = _usersRepository.LoadResetPasswordCode("SendCode_" + request.MailAddress);
            if (code == request.ResetCode)
            {// 生成用户数据，并保存到数据库 
                User user = _mapper.Map<User>(request);
                await _usersRepository.NewUserAsync(user);
                return new ApiResponse<RegisterResponse>(HttpStatusCode.OK, "Successful registration", null).Result();
            }
            return new ApiResponse<RegisterResponse>(HttpStatusCode.NotFound, "Incorrect verification code", null).Result();
        }
        public async Task<ActionResult<SendCodeResponse>> SendCodeAsync([FromBody] SendCodeRequest request)
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
            "The verification code has been sent to the specified email address", null).Result();
        }
        public async Task<ActionResult<UpdateUserInfoResponse>> UpdateUserInfoAsync(UpdateUserInfoRequest request)
        {
            var claimsIdentity = _httpContextAccessor?.HttpContext?.User.Identity as ClaimsIdentity;
            var userId = claimsIdentity!.FindFirst(KeyName.USER_ID)?.Value;
            User? user = await _usersRepository.GetUserByIdAsync(uint.Parse(userId!));

            await _usersRepository.UpdateUserAsync(user!);
            return new ApiResponse<UpdateUserInfoResponse>(null).Result();
        }

        public async Task<ActionResult<SendResetPasswordCodeResponse>> SendResetPasswordCodeAsync(SendResetPasswordCodeRequest request)
        {
            var user = await _usersRepository.GetUserByUsernameAsync(request.Email);
            if (user != null)
            {
                string toEmail = request.Email;
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
                "The verification code has been sent to the specified email address", null).Result();
            }
            else
            {
                return new ApiResponse<SendResetPasswordCodeResponse>(HttpStatusCode.NotFound, "Email not found", null).Result();
            }

        }

        public async Task<ActionResult<CodeResetPasswordResponse>> CodeResetPasswordAsync(CodeResetPasswordRequest request)
        {
            string code = _usersRepository.LoadResetPasswordCode(request.Email);
            if (code == request.ResetCode)
            {
                var user = await _usersRepository.GetUserByUsernameAsync(request.Email);
                if (user != null)
                {
                    user.Password = StringHelper.HashPassword(request.NewPassword);
                    await _usersRepository.UpdateUserAsync(user);
                    return new ApiResponse<CodeResetPasswordResponse>(null).Result();
                }
                else
                {
                    return new ApiResponse<CodeResetPasswordResponse>(HttpStatusCode.NotFound, "User not found", null).Result();
                }
            }
            else
            {
                return new ApiResponse<CodeResetPasswordResponse>(HttpStatusCode.NotFound, "Verification code error", null).Result();
            }
        }

        /// <summary>
        /// 检查邮箱是否存在
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CheckMailExist(string? mail)
        {
            if (string.IsNullOrEmpty(mail))
            {
                return true;
            }
            var has = await _usersRepository.CheckIfValueExistsAsync("Users", "MailAddress", mail);
            return has;
        }
    }
}
