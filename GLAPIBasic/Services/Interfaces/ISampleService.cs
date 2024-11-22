using GLAPIBasic.Models;

namespace GLAPIBasic.Services.Interfaces
{
    public interface ISampleService
    {
        Logininfo GetLogininfo(string username, string password);
    }
}
