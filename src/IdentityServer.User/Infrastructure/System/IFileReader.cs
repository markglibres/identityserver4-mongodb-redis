using System.Threading.Tasks;

namespace IdentityServer.Management.Infrastructure.Abstractions
{
    public interface IFileReader
    {
        Task<string> ReadContents(string filename);
    }
}
