using IdentityServer.Extensions;
using IdentityServer.Management;
using IdentityServer.Management.Extensions;
using IdentityServer.Management.Users;
using IdentityServer.Seeders;
using IdentityServer.Web.Apis;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IdentityServer_ApplicationUser = IdentityServer.Management.Users.ApplicationUser;

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
            services.AddControllers().AddIdentityServerUserApi();

            services.AddIdentityServerMongoDb(provider =>
                    new DefaultCorsPolicyService(provider.GetService<ILogger<DefaultCorsPolicyService>>())
                    {
                        AllowAll = true
                    })
                .AddRedisCache()
                .AddDeveloperSigningCredential()
                .AddIdentityServerUser<ApplicationUser, ApplicationRole>()
                .AddIdentityServerAudience()
                .SeedUsers<ApplicationUser, SeedUsers<ApplicationUser>>()
                .SeedClients<ApiClients>()
                .SeedClients<IdentityServerClients>()
                .SeedApiResources<UsersApiResource>()
                .SeedApiScope<UsersApiScopes>()
                .SeedIdentityResource<SeedIdentityResources>();

            services.AddSwashbuckle();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwashbuckle();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
                endpoints.MapControllers();
            });


        }
    }
}
