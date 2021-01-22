using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace IdentityServer.Users.Management.Infrastructure.System
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
