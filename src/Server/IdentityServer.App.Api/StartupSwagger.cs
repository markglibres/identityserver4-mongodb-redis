using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

namespace IdentityServer.App.Api
{
    public static class StartupSwagger
    {
        public static IServiceCollection AddSwashbuckle(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("IdentityUserV1", new OpenApiInfo
                {
                    Title = "Identity User",
                    Version = "v1"
                });

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        ClientCredentials = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri("http://localhost:5000"),
                            TokenUrl = new Uri("http://localhost:5000/connect/token"),
                            RefreshUrl = new Uri("http://localhost:5000/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "myapi.access", "API access" }
                            }
                        }
                    },
                    In = ParameterLocation.Header
                });
                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });
            services.AddSwaggerGenNewtonsoftSupport();

            return services;
        }

        public static IApplicationBuilder UseSwashbuckle(this IApplicationBuilder applicationBuilder)
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
