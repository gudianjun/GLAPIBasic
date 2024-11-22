using GLAPIBasic.Models;

namespace GLAPIBasic.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task SaveLoginInfoAsync(int userId, string audience, string token);
    }
}
