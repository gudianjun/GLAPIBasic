using FluentValidation;
using GLAPIBasic.DTOs;
using GLAPIBasic.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace GLAPIBasic.Validations
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        private readonly IUsersService _userService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RegisterRequestValidator> _logger;
        private readonly IMemoryCache _memoryCache;
        public RegisterRequestValidator(IConfiguration configuration, IUsersService userService
            , ILogger<RegisterRequestValidator> logger, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _userService = userService;
            _configuration = configuration;

            // 邮件地址也不存在。
            // 当mailAddress不为空时，验证mailAddress是否存在 
            RuleFor(x => x.UserName).Must((x, cancellation) =>
            {
                bool has = _userService.CheckMailExist(x.UserName).GetAwaiter().GetResult();
                return !has;
            }).WithMessage("UserName already exists");
        }
    }
}
