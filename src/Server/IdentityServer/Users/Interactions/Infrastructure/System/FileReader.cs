using System.IO;
using System.Threading.Tasks;

namespace IdentityServer.Users.Interactions.Infrastructure.System
{
    public class FileReader : IFileReader
    {
        public async Task<string> ReadContents(string filename)
        {
            var file = new FileStream(filename, FileMode.Open);
            using var reader = new StreamReader(file);
            return await reader.ReadToEndAsync();
        }
    }
}