using GLAPIBasic.DTOs;
using GLAPIBasic.Models;

namespace GLAPIBasic.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        void SaveLoginInfo(long userId, string audience, string token);
        UserTokenInfo? LoadLoginInfo(long userId);
    }
}
