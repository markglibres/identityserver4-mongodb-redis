using System.Threading.Tasks;

namespace IdentityServer.Users.Management.Infrastructure.System
{
    public interface IFileReader
    {
        Task<string> ReadContents(string filename);
    }
}
