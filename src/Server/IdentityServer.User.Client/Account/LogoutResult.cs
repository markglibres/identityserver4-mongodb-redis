using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace IdentityServer.User.Client.Account
{
    public class LogoutResult : ActionResult
    {
        public override async Task ExecuteResultAsync(ActionContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.HttpContext.Response.Redirect("/");
                return;
            };

            var schemes = context
                .HttpContext.RequestServices.GetRequiredService<IOptions<AuthenticationOptions>>()
                .Value
                .Schemes
                .Select(builder => builder.Name)
                .Where(scheme => !string.IsNullOrWhiteSpace(scheme))
                .ToList();

            if (!schemes.Any())
            {
                await context.HttpContext.SignOutAsync();
                return;
            };

            schemes.ForEach(async scheme =>
            {
                try
                {
                    await context.HttpContext.SignOutAsync(scheme);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });
        }
    }
}
