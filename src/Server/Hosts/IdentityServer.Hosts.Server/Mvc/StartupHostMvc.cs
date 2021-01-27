using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.HostServer.Mvc
{
    public static class StartupHostMvc
    {
        public static IMvcBuilder AddIdentityServerUserManagementMvc(this IMvcBuilder mvcBuilder)
        {
            var builder = mvcBuilder.AddApplicationPart(typeof(StartupHostMvc).Assembly);
            return builder;
        }
    }
}
