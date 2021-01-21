using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Users.Management.Application.Abstractions;
using IdentityServer.Users.Management.Infrastructure.System;

namespace IdentityServer.Users.Management.Infrastructure.Templates
{
    public class EmailTemplate : IEmailTemplate
    {
        private readonly ITemplateParser _templateParser;
        private readonly IFileReader _fileReader;
        private IEnumerable<ITemplateProvider> _templateProviders;

        public EmailTemplate(IEnumerable<ITemplateProvider> templateProviderses,
            ITemplateParser templateParser,
            IFileReader fileReader)
        {
            _templateProviders = templateProviderses;
            _templateParser = templateParser;
            _fileReader = fileReader;
        }

        public async Task<string> Generate(object data, Action<EmailTemplateOptions> options)
        {
            var templateOptions = new EmailTemplateOptions();
            options(templateOptions);

            return await Generate(data, templateOptions);
        }

        public async Task<string> Generate(object data, EmailTemplateOptions options)
        {
            var provider =
                _templateProviders.FirstOrDefault(templateProvider => templateProvider.CanHandle(options));

            if (provider == null) return string.Empty;

            var templateContent = await provider.GetContents(options);

            var parsedContent = await _templateParser.Parse(templateContent, data);
            return parsedContent;
        }
    }
}
