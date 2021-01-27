using IdentityServer.Extensions;
using IdentityServer.Hosts.React.Resources;
using IdentityServer.Management;
using IdentityServer.Management.Extensions;
using IdentityServer.Management.Users;
using IdentityServer.Seeders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer.Hosts.React
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllersWithViews()
                .AddIdentityServerUserApi();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddIdentityServerUserAuthentication();

            services
                .AddIdentityServerMongoDb()
                .AddRedisCache()
                .AddDeveloperSigningCredential()
                .AddIdentityServerUser<ApplicationUser, ApplicationRole>()
                .SeedUsers<ApplicationUser, SeedUsers<ApplicationUser>>()
                .SeedClients<ApiClients>()
                .SeedClients<IdentityServerClients>()
                .SeedApiResources<UsersApiResource>()
                .SeedApiScope<UsersApiScopes>()
                .SeedIdentityResource<SeedIdentityResources>();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment()) spa.UseReactDevelopmentServer("start");
            });
        }
    }
}
