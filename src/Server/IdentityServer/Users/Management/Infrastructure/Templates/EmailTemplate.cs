using System;
using System.Threading.Tasks;
using IdentityServer.Users.Management.Application.Abstractions;
using IdentityServer.Users.Management.Infrastructure.System;

namespace IdentityServer.Users.Management.Infrastructure.Templates
{
    public class EmailTemplate : IEmailTemplate
    {
        private readonly ITemplateProvider _templateProvider;
        private readonly ITemplateParser _templateParser;
        private readonly IFileReader _fileReader;

        public EmailTemplate(ITemplateProvider templateProvider,
            ITemplateParser templateParser,
            IFileReader fileReader)
        {
            _templateProvider = templateProvider;
            _templateParser = templateParser;
            _fileReader = fileReader;
        }

        public async Task<string> Generate(object data, Action<EmailTemplateOptions> options)
        {
            var templateOptions = new EmailTemplateOptions();
            options(templateOptions);

            var templateContent = templateOptions.Content;
            if (string.IsNullOrWhiteSpace(templateContent) && !string.IsNullOrWhiteSpace(templateOptions.File))
            {
                templateContent = await _templateProvider.GetContents(templateOptions.File);
                if (string.IsNullOrWhiteSpace(templateContent))
                {
                    templateContent = await _fileReader.ReadContents(templateOptions.File);
                }
            }

            var parsedContent = await _templateParser.Parse(templateContent, data);
            return parsedContent;
        }
    }
}
