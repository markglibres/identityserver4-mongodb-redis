using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer.Users.Interactions.Application.Abstractions;

namespace IdentityServer.Users.Interactions.Infrastructure.Templates
{
    public class EmbeddedResourceTemplateProvider : ITemplateProvider
    {
        public async Task<string> GetContents(EmailTemplateOptions templateOptions)
        {
            if (!CanHandle(templateOptions)) return string.Empty;

            var filename = templateOptions.File;
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly
                .GetManifestResourceNames()
                .SingleOrDefault(f => f.EndsWith(filename, StringComparison.OrdinalIgnoreCase));

            if (string.IsNullOrWhiteSpace(resourceName))
                throw new FileNotFoundException(
                    $"The embedded resource {filename} can't be found within {assembly.GetName().Name} assembly");

            await using var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null) throw new EndOfStreamException($"No stream for {resourceName}");
            using var streamReader = new StreamReader(stream);

            return await streamReader.ReadToEndAsync();
        }

        public bool CanHandle(EmailTemplateOptions options)
        {
            return options.FileStorageType == FileStorageTypes.Embedded;
        }
    }
}