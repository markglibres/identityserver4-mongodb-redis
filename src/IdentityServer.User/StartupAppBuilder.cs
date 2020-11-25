using Microsoft.AspNetCore.Builder;

namespace IdentityServer.Management
{
    public static class StartupBuilder
    {
        public static IApplicationBuilder UseIdentityServerUser(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseSwagger();
            applicationBuilder.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("IdentityUserV1/swagger.json", "Identity User v1");

                options.OAuthClientId("swagger");
                options.OAuthClientSecret("hardtoguess");
                options.OAuthAppName("BizzPo API");
            });

            return applicationBuilder;
        }
    }
}
