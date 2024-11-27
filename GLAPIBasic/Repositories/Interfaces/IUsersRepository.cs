using GLAPIBasic.Models;

namespace GLAPIBasic.Repositories.Interfaces
{
    public interface IUsersRepository
    {
        Task<UserInfo?> GetUserInfoForUserNameAsync(string userName);
 
        Task<UserInfo?> GetUserByIdAsync(long userId); 

        Task NewUserAsync(UserInfo user);

        Task<int> UpdateUserAsync(UserInfo user); 
 
        void SaveResetPasswordCode(string email, string code);

        string LoadResetPasswordCode(string email);

        Task<bool> CheckIfValueExistsAsync(string tableName, string columnName, object value);

    }
}
