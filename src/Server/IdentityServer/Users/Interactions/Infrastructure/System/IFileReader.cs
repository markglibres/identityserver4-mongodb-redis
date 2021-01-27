using System.Threading.Tasks;

namespace IdentityServer.Users.Interactions.Infrastructure.System
{
    public interface IFileReader
    {
        Task<string> ReadContents(string filename);
    }
}