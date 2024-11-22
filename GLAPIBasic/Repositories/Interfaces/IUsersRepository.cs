using GLAPIBasic.Models;

namespace GLAPIBasic.Repositories.Interfaces
{
    public interface IUsersRepository
    {
        Task<User?> GetUserByUsernameAsync(string username);

        Task<User?> GetUserInfoForUserNameAsync(string userName);
 
        Task<User?> GetUserByIdAsync(uint userId);
        Task SaveUserAsync(User user);

        Task NewUserAsync(User user);
        Task<int> UpdateUserAsync(User user);
        Task ChangePasswordAsync(string userId, string newPassword);
 
        void SaveResetPasswordCode(string email, string code);
        string LoadResetPasswordCode(string email);

        Task<bool> CheckIfValueExistsAsync(string tableName, string columnName, object value);

    }
}
