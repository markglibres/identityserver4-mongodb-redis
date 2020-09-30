![Build Master](https://github.com/markglibres/identityserver4-mongodb-redis/workflows/Build%20Master/badge.svg?branch=master) [![NuGet Badge](https://buildstats.info/nuget/BizzPo.IdentityServer)](https://www.nuget.org/packages/BizzPo.IdentityServer/)

# IdentityServer4 with MongoDb & Redis cache
[IdentityServer](https://github.com/IdentityServer/IdentityServer4) is a free, open source [OpenID Connect](http://openid.net/connect/) and [OAuth 2.0](https://tools.ietf.org/html/rfc6749) framework for [ASP.NET](http://asp.net/) Core and is officially [certified](https://openid.net/certification/) by the [OpenID Foundation](https://openid.net/) and thus spec-compliant and interoperable.

This library does the heavy plumbing for IdentityServer4 with MongoDb and Redis cache. 
## Features
* MongoDb store 
* Redis cache as provided by [IdentityServer4.Contrib.RedisStore](https://github.com/AliBazzi/IdentityServer4.Contrib.RedisStore)
* Easy configuration for Resource Owner Password grant
* Comes with an optional seed data that you can use to start testing IdentityServer4
* Easy way to seed data

## Installation
1. Install NuGet package [BizzPo.IdentityServer](https://www.nuget.org/packages/BizzPo.IdentityServer/)
2. In `appsettings.json`, add configuration for mongodb and redis
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