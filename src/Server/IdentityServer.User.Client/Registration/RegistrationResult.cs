using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.User.Client.Registration
{
    public class RegistrationResult : ActionResult
    {
        public override async Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null) throw new ArgumentException(nameof(context));

            var userInteractionService =
                context.HttpContext.RequestServices.GetRequiredService<IUserInteractionService>();

            var registrationContext = await userInteractionService.GetRegistrationContext();

            context.HttpContext.Response.Redirect(registrationContext.RegistrationUrl);

            // //redirect with post
            // var response = context.HttpContext.Response;
            // response.Clear();
            // var sb = new System.Text.StringBuilder();
            // sb.Append("<html>");
            // sb.AppendFormat("<body onload='document.forms[0].submit()'>");
            // sb.AppendFormat("<form action='{0}' method='post'>", registrationContext.RegistrationUrl);
            // sb.AppendFormat("<input type='hidden' name='token' value='{0}'>", registrationContext.Token);
            // sb.Append("</form>");
            // sb.Append("</body>");
            // sb.Append("</html>");
            // await response.WriteAsync(sb.ToString());

        }
    }
}
