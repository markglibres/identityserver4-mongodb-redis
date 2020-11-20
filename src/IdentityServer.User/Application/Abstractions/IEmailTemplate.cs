using System;
using System.Threading.Tasks;

namespace IdentityServer.Management.Application.Abstractions
{
    public interface IEmailTemplate
    {
        Task<string> Generate(object data, Action<EmailTemplateOptions> options);
    }

    public class EmailTemplateOptions
    {
        public string File { get; set; }
        public string Content { get; set; }
    }
}
