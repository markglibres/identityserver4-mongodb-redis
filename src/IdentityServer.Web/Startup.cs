using IdentityServer.Services;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IdentityServer_ApplicationUser = IdentityServer.Web.ApplicationUser;

namespace IdentityServer.Web
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ISeeder<ApplicationUser>, SeedUsers>();
            services.AddSingleton<ISeeder<Client>, SeedClients>();
            services.AddSingleton<ISeeder<ApiResource>, SeedApiResources>();
            services.AddSingleton<ISeeder<ApiScope>, SeedApiScopes>();
            services.AddSingleton<ISeeder<IdentityResource>, SeedIdentityResources>();

            services.AddMongoDbIdentityServer<ApplicationUser, ApplicationRole, ApplicationProfile>(
                options => { options.IssuerUri = "http://localhost:5000"; },
                provider =>
                    new DefaultCorsPolicyService(provider.GetService<ILogger<DefaultCorsPolicyService>>())
                    {
                        AllowAll = true
                    }, 
                builder =>
                {
                    builder.AddDeveloperSigningCredential() // use a valid signing cert in production
                        //.AddInMemoryResources()
                        //.AddInMemoryClients()
                        .AddMongoDbResources()
                        .AddMongoDbClientStore()
                        .AddRedisCaching();
                });
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