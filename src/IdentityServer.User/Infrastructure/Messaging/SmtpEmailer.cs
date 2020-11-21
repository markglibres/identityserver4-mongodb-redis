using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using IdentityServer.Management.Application.Abstractions;
using Microsoft.Extensions.Options;

namespace IdentityServer.Management.Infrastructure.Messaging
{
    public class SmtpEmailer : IEmailer
    {
        private readonly SmtpConfig _options;

        public SmtpEmailer(IOptions<SmtpConfig> options)
        {
            _options = options.Value;
        }

        public async Task Send(string email, string subject, string content)
        {
            var client = new SmtpClient(_options.Host, _options.Port)
            {
                Credentials = new NetworkCredential(_options.Username,
                    _options.Password)
            };
            var to = new MailAddress(email);
            var from = new MailAddress(_options.SenderEmail, _options.SenderName);
            var message = new MailMessage(from, to)
            {
                Body = content,
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
                Subject = subject,
                SubjectEncoding = Encoding.UTF8
            };

            if(_options.IsSandBox) return;

            await client.SendMailAsync(message);
        }
    }
}
