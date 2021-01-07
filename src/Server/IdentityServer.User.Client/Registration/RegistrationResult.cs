using System;
using System.Threading.Tasks;
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
        }
    }
}
