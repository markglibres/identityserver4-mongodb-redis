using System.IO;
using System.Threading.Tasks;
using IdentityServer.Management.Infrastructure.Abstractions;

namespace IdentityServer.Management.Infrastructure.System
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
