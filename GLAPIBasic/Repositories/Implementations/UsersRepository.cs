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
    public class UsersRepository : IUsersRepository
    {
        private readonly IConfiguration _configuration;
        private readonly PgDbContext _context;
        private readonly ILogger<UsersRepository> _logger;
        private readonly IMemoryCache _memoryCache;
        public UsersRepository(IConfiguration configuration, PgDbContext context
            , ILogger<UsersRepository> logger, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }
 
        public async Task<UserInfo?> GetUserInfoForUserNameAsync(string userName)
        {
            var rtn = await _context.UserInfos.FirstOrDefaultAsync(x => x.Username == userName);
            // 通过UserName获取User表中的用户信息
            return rtn;
        }
 
        public Task<UserInfo?> GetUserByIdAsync(long userId)
        {
            var rtn = _context.UserInfos.FirstOrDefaultAsync(x => x.UserId == userId);
            return rtn;
        }
  
        public async Task NewUserAsync(UserInfo user)
        {
            // 新增User表中的用户信息
            await _context.UserInfos.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        public async Task<int> UpdateUserAsync(UserInfo user)
        {
            // 更新User表中的用户信息
            _context.UserInfos.Update(user);
            int count = await _context.SaveChangesAsync();
            return count;
        } 

        public void SaveResetPasswordCode(string email, string code)
        {
            // 保存验证码到内存中， 10分钟有效
            _memoryCache.Set(email, code.ToString(), TimeSpan.FromMinutes(10));
        }
        public string LoadResetPasswordCode(string email)
        {
            // 从内存中获取验证码
            if (_memoryCache.TryGetValue(email, out string? code))
            {
                return code!;
            }
            else
            {
                return string.Empty;
            }
        }


        public async Task<bool> CheckIfValueExistsAsync(string tableName, string columnName, object value)
        {
            // 获取 DbSet 属性
            var dbSetProperty = _context.GetType().GetProperty(tableName);
            if (dbSetProperty == null)
            {
                throw new ArgumentException($"Table '{tableName}' does not exist in the context.");
            }

            // 获取 DbSet 实例
            var dbSet = dbSetProperty.GetValue(_context) as IQueryable<object>;
            if (dbSet == null)
            {
                throw new ArgumentException($"Table '{tableName}' is not a valid DbSet.");
            }

            // 构建动态查询
            var parameter = Expression.Parameter(typeof(object), "x");
            var property = Expression.Property(Expression.Convert(parameter, dbSet.ElementType), columnName);
            var constant = Expression.Constant(value);
            var equal = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<object, bool>>(equal, parameter);

            // 执行查询
            return await dbSet.AnyAsync(lambda);
        }
         
  
    }
}
