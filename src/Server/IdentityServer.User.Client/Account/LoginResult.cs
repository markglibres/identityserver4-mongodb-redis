using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IdentityServer.User.Client.Account
{
    public class LoginResult : ActionResult
    {
        private readonly string _returnUrl;

        public LoginResult(string returnUrl = "")
        {
            _returnUrl = returnUrl;
        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.HttpContext.Response.Redirect("/");
                return;
            }

            ;

            var options = context
                .HttpContext.RequestServices.GetRequiredService<IOptions<AuthenticationOptions>>()
                .Value;

            await context.HttpContext.ChallengeAsync(options.DefaultChallengeScheme, new AuthenticationProperties
            {
                RedirectUri = _returnUrl
            });
        }
    }
}