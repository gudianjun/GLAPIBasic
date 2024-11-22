using GLAPIBasic.Data;
using GLAPIBasic.DTOs;
using GLAPIBasic.Enums;
using GLAPIBasic.Models;
using GLAPIBasic.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;

namespace GLAPIBasic.Repositories.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IConfiguration _configuration;
        private readonly PgDbContext _context;
        private readonly ILogger<AuthRepository> _logger;
        private readonly IMemoryCache _memoryCache;
        public AuthRepository(IConfiguration configuration, PgDbContext context
            , ILogger<AuthRepository> logger, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _configuration = configuration;
            _context = context;
        } 
        public Task SaveLoginInfoAsync(int userId, string audience, string session)
        {
            if (!_memoryCache.TryGetValue(userId, out UserTokenInfo? userTokenInfo))
            {
                userTokenInfo = new UserTokenInfo();
            } // 根据audience更新相应的Token值
            if (audience == Audience.Browser)
            {
                userTokenInfo!.BrowserSession = session;
            }
            else if (audience == Audience.Mobile)
            {
                userTokenInfo!.MobileSession = session;
            }
            // 保存到内存中
            _memoryCache.Set(userId, userTokenInfo);
            return Task.CompletedTask;
        }
    }
}
