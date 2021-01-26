using IdentityServer.Authorization.Extensions;
using IdentityServer.Authorization.Seeders;
using IdentityServer.Hosts.Mvc.Controllers;
using IdentityServer.Hosts.Mvc.Resources;
using IdentityServer.HostServer.Mvc;
using IdentityServer.Users.Authorization;
using IdentityServer.Users.Authorization.Services;
using IdentityServer.Users.Interactions;
using IdentityServer.Users.Management;
using IdentityServer.Users.Management.Application.Abstractions;
using IdentityServer.Users.Management.Configs;
using IdentityServer.Users.Seeders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer.Hosts.Mvc
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
                .AddIdentityServerUserInteraction()
                .AddIdentityServerUserManagement(config =>
                {
                    config.Scope = "users.management";
                    config.UserInteractions = new UserInteractionsConfig
                    {
                        CreateUser = "/Registration/CreateUser",
                        ConfirmUser = "/Registration/Confirm",
                        ResetPassword = "/Registration/ResetPassword",
                        LoginUrl = "/Account/Login",
                        LogoutUrl = "/Account/Logout"
                    };
                    config.Emails = new Emails
                    {
                        EmailConfirmation = new EmailConfig
                        {
                            Subject = "Confirm user registration",
                            TemplateOptions = new EmailTemplateOptions
                            {
                                File = "~/Templates/user-email-confirmation.html",
                                FileStorageType = FileStorageTypes.Local
                            }
                        },
                        ForgotPassword = new EmailConfig
                        {
                            Subject = "Reset password link",
                            TemplateOptions = new EmailTemplateOptions
                            {
                                File = "user-forgotpassword-request.html",
                                FileStorageType = FileStorageTypes.Embedded
                            }
                        }
                    };
                })
                .AddIdentityServerUserManagementMvc();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddIdentityServerUserAuthorization();

            services
                .AddIdentityServerMongoDb()
                .AddRedisCache()
                .AddDeveloperSigningCredential()
                .AddIdentityServerUserAuthorization<ApplicationUser, ApplicationRole>()
                .SeedUsers<ApplicationUser, SeedUsers<ApplicationUser>>()
                .SeedClients<ApiClients>()
                .SeedClients<IdentityServerClients>()
                .SeedApiResources<UsersApiResource>()
                .SeedApiScope<UsersApiScopes>()
                .SeedIdentityResource<SeedIdentityResources>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler("/Home/Error");

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
