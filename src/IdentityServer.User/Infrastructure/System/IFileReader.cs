using System.Threading.Tasks;

namespace IdentityServer.Management.Infrastructure.System
{
    public interface IFileReader
    {
        Task<string> ReadContents(string filename);
    }
}
