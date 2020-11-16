using IdentityServer.Extensions;
using IdentityServer.Seeders;
using IdentityServer.Users;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IdentityServer_ApplicationUser = IdentityServer.Users.ApplicationUser;

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
            services.AddControllers();

            services.AddIdentityServerMongoDb(provider =>
                    new DefaultCorsPolicyService(provider.GetService<ILogger<DefaultCorsPolicyService>>())
                    {
                        AllowAll = true
                    })
                .AddRedisCache()
                .AddDeveloperSigningCredential()
                .AddIdentityServerUser<IdentityServer_ApplicationUser, ApplicationRole>()
                .SeedUsers<IdentityServer_ApplicationUser, SeedUsers<IdentityServer_ApplicationUser>>()
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