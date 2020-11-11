using System.Collections.Generic;
using Identity;
using Identity.Common;
using Identity.Common.Seeders;
using Identity.Common.Users;
using IdentityServer.Extensions;
using IdentityServer.Seeders;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IdentityServer_ApplicationUser = Identity.Common.Users.ApplicationUser;

namespace IdentityServer.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddIdentity();
            
            services.AddIdentityServerMongoDb(provider =>
                    new DefaultCorsPolicyService(provider.GetService<ILogger<DefaultCorsPolicyService>>())
                    {
                        AllowAll = true
                    })
                .AddRedisCache()
                .AddDeveloperSigningCredential()
                .AddResourceOwnerPassword<IdentityServer_ApplicationUser, ApplicationRole>()
                .SeedUsers<ApplicationUser, SeedUsers<ApplicationUser>>()
                .SeedClients<SeedClients>()
                .SeedApiResources<SeedApiResources>()
                .SeedApiScope<SeedApiScopes>()
                .SeedIdentityResource<SeedIdentityResources>();

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
                endpoints.MapControllers();
            });
        }
    }
}