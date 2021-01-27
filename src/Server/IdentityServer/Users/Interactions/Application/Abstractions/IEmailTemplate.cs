using System;
using System.Threading.Tasks;

namespace IdentityServer.Users.Interactions.Application.Abstractions
{
    public interface IEmailTemplate
    {
        Task<string> Generate(object data, Action<EmailTemplateOptions> options);
        Task<string> Generate(object data, EmailTemplateOptions options);
    }

    public class EmailTemplateOptions
    {
        public string File { get; set; }
        public string FileStorageType { get; set; }
    }

    public static class FileStorageTypes
    {
        public const string Embedded = "embedded";
        public const string Local = "local";
    }
}