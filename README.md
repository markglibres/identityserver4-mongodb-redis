


![Build Master](https://github.com/markglibres/identityserver4-mongodb-redis/workflows/Build%20Master/badge.svg?branch=master) [![NuGet Badge](https://buildstats.info/nuget/BizzPo.IdentityServer)](https://www.nuget.org/packages/BizzPo.IdentityServer/) [![Docker Hub](https://img.shields.io/badge/docker-ready-blue.svg)](https://hub.docker.com/r/bizzpo/identityserver4/)

# IdentityServer4 with MongoDb & Redis cache
"[IdentityServer](https://github.com/IdentityServer/IdentityServer4) is a free, open source [OpenID Connect](http://openid.net/connect/) and [OAuth 2.0](https://tools.ietf.org/html/rfc6749) framework for [ASP.NET](http://asp.net/) Core and is officially [certified](https://openid.net/certification/) by the [OpenID Foundation](https://openid.net/) and thus spec-compliant and interoperable."

This library does the heavy plumbing for IdentityServer4 with MongoDb and Redis cache. Developers can quickly spin up a docker image and start developing their apps without paying for $$$ on authorization service.

## Features
* MongoDb store 
* Redis cache as provided by [IdentityServer4.Contrib.RedisStore](https://github.com/AliBazzi/IdentityServer4.Contrib.RedisStore)
* Easy configuration for Resource Owner Password grant
* Comes with an optional seed data that you can use to start testing IdentityServer4
* Easy way to seed data

## Contents
[Run sample IdentityServer with docker hub image](#run-sample-identityserver-with-docker-hub-image)

[Run sample IdentityServer with local docker](#run-sample-identityserver-with-local-docker)

[Installation via NuGet](#installation-via-nuget)

[Configure seed data](#configure-seed-data)

[Configure for Resource Owner Password grant](#configure-for-resource-owner-password-grant)

[Configure seed users](#configure-seed-users)

[How to seed data](#built-in-seed-data)

[Sample on how to configure API client](#sample-on-how-to-configure-api-client)

## Run sample IdentityServer with docker hub image
1. Create docker-compose file i.e. `docker-compose-identity.yaml` and pull image from `bizzpo/identityserver4`. For example:
    ```yaml
    version: '3.4'
    services:
        identity:
            image: bizzpo/identityserver4
            environment:
                - Environment=Development
                - "Identity__Mongo__ConnectionString=mongodb://root:foobar@mongodb:27017/?readPreference=primaryPreferred&appname=identityserver"
                - Identity__Mongo__Database=Identity
                - Identity__Redis__ConnectionString=redis
                - Identity__Redis__Db=-1
                - Identity__Redis__Prefix=identity
            container_name: identity
            depends_on:
                - redis
                - mongodb
            ports:
                - 5000:80
            networks:
                - identity
            restart: always
        redis:
            image: bitnami/redis
            ports:
                - 6379:6379
            environment:
                - ALLOW_EMPTY_PASSWORD=yes
            networks:
                - identity
            restart: always
        mongodb:
            image: mongo:4.2.8
            environment:
                - MONGO_INITDB_ROOT_USERNAME=root
                - MONGO_INITDB_ROOT_PASSWORD=foobar
            ports:
                - 27017:27017
            networks:
                - identity
            restart: always
    networks:
        identity:
            driver: bridge
    ```
2. Run docker-compose file
    ```bash
    docker-compose -f docker-compose-identity.yaml up
    ```
3. Execute get token with the following details:
    ```bash
    *    method = POST
    *    url = http://localhost:5000/connect/token
    *    grant_type = password
    *    username = dev
    *    password = hardtoguess
    *    scope = myapi.access openid offline_access
    *    client_id = spaWeb
    *    client_secret = hardtoguess
    ```
Click [here](https://github.com/markglibres/identityserver4-mongodb-redis/tree/master/src/IdentityServer/Seeders) to see more seeded sample clients and grant types

## Run sample IdentityServer with local docker
1. Clone this repository
    ```bash
    git clone git@github.com:markglibres/identityserver4-mongodb-redis.git
    ```
2. Build docker images
    ```bash
    docker-compose -f src/docker-compose.yaml build
    ```
3. Run docker containers
    ```bash
    docker-compose -f src/docker-compose.yaml up
    ```
4. Install Postman UI and import the postman folder `/postman`
5. On Postman UI, run sample Login, Logout, Refresh Token and Get User endpoints with `identity_docker` environment

## Installation via NuGet
1. Create empty dotnetcore API project
2. Install NuGet package [BizzPo.IdentityServer](https://www.nuget.org/packages/BizzPo.IdentityServer/)
3. In `appsettings.json`, add configuration for mongodb and redis
    ```json
    "Identity": {
        "Authority": "http://localhost:5000", /* url of your identity server */
        "Mongo": {
            "ConnectionString": "mongodb://root:foobar@localhost:27017/?readPreference=primaryPreferred&appname=identityserver", /*connection string for your mongodb server*/
            "Database": "Identity"
        },
        "Redis": {
            "ConnectionString": "localhost",
            "Db": -1,
            "Prefix": "identity"
        }
    }
    ```
4. In `Startup.cs -> ConfigureServices`, register MongoDb and Redis caching
    ```csharp
    services.AddIdentityServerMongoDb(provider =>
        new DefaultCorsPolicyService(provider.GetService<ILogger<DefaultCorsPolicyService>>())
        {
            AllowAll = true,
            //configure allowed origins
            //AllowedOrigins = new List<string> { }
        })
        .AddRedisCache()
        .AddDeveloperSigningCredential();
    ```    
Build the project and run. You now have your first IdentityServer4 running, but to start generating tokens, we have to configure the clients, api resources, scopes and identity resources.

## Configure seed data
1. In `Program.cs`, initialize seeder service
    ```csharp
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        await host.Services.Initialize();
        await host.RunAsync();
    }
    ```
2. Using interface `ISeeder<T>`, create seeder class for types Clients, ApiResources, ApiScopes and IdentityResources. For example:
    ```csharp
    public class SeedApiScopes : ISeeder<ApiScope>
    {
        public IEnumerable<ApiScope> GetSeeds() => new[]
        {
            new ApiScope("myapi.access", "Access API Backend")
        }
    }
    ```
3.  In `Startup.cs`, register the seeder classes
    ```csharp
    services.AddIdentityServerMongoDb(provider =>
        // codes removed for brevity
        .SeedClients<SeedClients>()
        .SeedApiResources<SeedApiResources>()
        .SeedApiScope<SeedApiScopes>()
        .SeedIdentityResource<SeedIdentityResources>();
    ```
4. Build and run the project to generate tokens.

## Configure for Resource Owner Password grant
1. Create `User` class and inherit from `IdentityUser`, or you can implement from partial class `ApplicationUser`
2. Create `Role` class and inherit from `IdentityRole`, or you can implement from partial class `ApplicationRole`
3. In `Startup.cs`, configure Resource Owner Password grant type
    ```csharp
    services.AddIdentityServerMongoDb(provider =>
        // codes removed for brevity
        .AddResourceOwnerPassword<ApplicationUser, ApplicationRole>();
    ```
4. Build and run the project, but you need to populate your users store to start generating token. See next steps on how to seed users.

## Configure seed users
1. In `Program.cs`, initialize seeder service for users
    ```csharp
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        // codes removed for brevity
        await host.Services.Initialize<ApplicationUser>();
        await host.RunAsync();
    }
    ```
2. Create seeder class for users and inherit from `ISeeder<T>` where T is your user type. i.e. ApplicationUser. For example:
    ```csharp
    public class SeedUsers : ISeeder<ApplicationUser>
    {
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        public SeedUsers(IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }
        
        public IEnumerable<T> GetSeeds() => new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "dev",
                PasswordHash = _passwordHasher.HashPassword(null, "hardtoguess"),
                EmailConfirmed = true
            }
        }
    }
    ```
3. In `Startup.cs`, register the seeder class
    ```csharp
    services.AddIdentityServerMongoDb(provider =>
        // codes removed for brevity
        .AddResourceOwnerPassword<ApplicationUser, ApplicationRole>()
        .SeedUsers<ApplicationUser, SeedUsers>()
    ```
4.  Build and run the project. You should now be able to generate tokens using resource owner password grant type.

## Built-in seed data
The project contains an optional seed data that any developer can start generating tokens. Add the whole block on configure services
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddIdentityServerMongoDb(provider =>
        new DefaultCorsPolicyService(provider.GetService<ILogger<DefaultCorsPolicyService>>())
        {
            AllowAll = true,
        }
        .AddRedisCache()
        .AddDeveloperSigningCredential()
        .AddResourceOwnerPassword<IdentityServer_ApplicationUser, ApplicationRole>()
        .SeedUsers<ApplicationUser, SeedUsers<ApplicationUser>>()
        .SeedClients<SeedClients>()
        .SeedApiResources<SeedApiResources>()
        .SeedApiScope<SeedApiScopes>()
        .SeedIdentityResource<SeedIdentityResources>();
}
```

Seeded values can be found [here](https://github.com/markglibres/identityserver4-mongodb-redis/tree/master/src/IdentityServer/Seeders)

## Sample on how to configure API client
Steps below is for a sample C# API project
1. Create an empty API project
2. Install the following NuGet packages:
	```csharp
	* Microsoft.AspNetCore.Authentication.JwtBearer
	* IdentityModel.AspNetCore.OAuth2Introspection
	* IdentityModel.AspNetCore.AccessTokenValidation
	```
3. Configure authorization with JWT bearer token:
	```csharp
	services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) 
		.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => { 
			options.Authority = "http://localhost:5000"; 
			options.Audience = "myapi"; 
			options.RequireHttpsMetadata = false; 
			//if token does not contain a dot, it is a reference token 
			//if it's a reference token, will forward it to introspection
			options.ForwardDefaultSelector = Selector.ForwardReferenceToken("introspection"); 
		});
	```
4. Configuration authorization with introspection (using reference token instead of access token)
	```csharp
	services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => { 
			// codes removed for brevity
		})
		.AddOAuth2Introspection("introspection", options =>
	    {
	        options.Authority = "http://localhost:8989";
	        options.ClientId = "myapi";
	        // the API secret value configured in previous step
	        options.ClientSecret = "hardtoguess"; 
	        // optional: use non-ssl for discovery endpoint, by default uses SSL
	        options.DiscoveryPolicy = new DiscoveryPolicy
	        {
	            RequireHttps = false
	        };
    });
	```
5.  Configure pipeline to use authorization.
	```csharp
	app.UseIdentityServer(); 
	app.UseAuthentication();  
	app.UseAuthorization();
	```
6. Authorize controllers and set the default authorization scheme
	```csharp
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class MyApiController : ControllerBase
	{
	}
	```
