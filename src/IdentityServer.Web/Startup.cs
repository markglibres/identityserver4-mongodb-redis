using IdentityServer.Authentication;
using IdentityServer.Authorization;
using IdentityServer.Repositories;
using IdentityServer.Services;
using IdentityServer.Stores;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IdentityServer_ApplicationUser = IdentityServer.Web.ApplicationUser;

namespace IdentityServer.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentityServerMongoDb(options =>
                {
                    options.IssuerUri = "http://localhost:5000";
                }, provider => new DefaultCorsPolicyService(provider.GetService<ILogger<DefaultCorsPolicyService>>())
                {
                    AllowAll = true
                })
                .AddRedisCaching()
                .AddClients<SeedClients>()
                .AddApiResources<SeedApiResources>()
                .AddApiScope<SeedApiScopes>()
                .AddIdentityResource<SeedIdentityResources>()
                .AddDeveloperSigningCredential()
                .AddResourceOwnerPassword<ApplicationUser, ApplicationRole>()
                .AddResourceOwnerPasswordUsers<ApplicationUser, SeedUsers<ApplicationUser>>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
            });
        }
    }
}