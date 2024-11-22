using GLAPIBasic.Models;
using GLAPIBasic.Services.Interfaces;

namespace GLAPIBasic.Services.Implementations
{
    public class SampleService : ISampleService
    {
        public SampleService()
        {
        }

        public Logininfo GetLogininfo(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
