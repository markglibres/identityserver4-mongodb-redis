using System;
using System.Threading.Tasks;
using HandlebarsDotNet;

namespace IdentityServer.Management.Infrastructure.Templates
{
    public class HandleBarsTemplateParser : ITemplateParser
    {
        public Task<string> Parse(string templateString, object model)
        {
            RegisterHelpers();
            var template = Handlebars.Compile(templateString);
            var result = template(model);

            var preMailer = PreMailer.Net.PreMailer.MoveCssInline(
                result,
                stripIdAndClassAttributes: true);

            return Task.FromResult(preMailer.Html);
        }

        private static void RegisterHelpers()
        {
            Handlebars.RegisterHelper("DateTimeFormat", (output, context, data) =>
            {
                var dateTime = new DateTime(((DateTimeOffset) data[0]).Ticks, DateTimeKind.Utc);
                var formatString = data[1];

                output.WriteSafeString(dateTime.ToString(formatString.ToString()));
            });
        }
    }
}
