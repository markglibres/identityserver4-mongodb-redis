using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer.Users.Interactions.Application.Abstractions;
using IdentityServer.Users.Interactions.Infrastructure.System;

namespace IdentityServer.Users.Interactions.Infrastructure.Templates
{
    public class LocalFileTemplateProvider : ITemplateProvider
    {
        private readonly IFileReader _fileReader;

        public LocalFileTemplateProvider(IFileReader fileReader)
        {
            _fileReader = fileReader;
        }

        public async Task<string> GetContents(EmailTemplateOptions options)
        {
            var filePath = options.File;
            if (!Path.IsPathRooted(filePath))
            {
                var buildDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                filePath = Path.Combine(buildDir ?? string.Empty, filePath.Replace("~/", string.Empty));
            }

            var content = await _fileReader.ReadContents(filePath);
            return content;
        }

        public bool CanHandle(EmailTemplateOptions options)
        {
            return options.FileStorageType == FileStorageTypes.Local;
        }
    }
}