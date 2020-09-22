# IdentityServer4 using MongoDb with Redis cache

[IdentityServer](https://github.com/IdentityServer/IdentityServer4) is a free, open source [OpenID Connect](http://openid.net/connect/) and [OAuth 2.0](https://tools.ietf.org/html/rfc6749) framework for [ASP.NET](http://ASP.NET) Core and is officially [certified](https://openid.net/certification/) by the [OpenID Foundation](https://openid.net/) and thus spec-compliant and interoperable.

IdentityServer4 supports other grant types, but for this tutorial we will use Resource Owner Password Grant (ROPG) for the following reasons:

1.  Our mobile / web app will have its own login screen for better UX experience.    
2.  We maintain both the frontend and backend services. ROPG is not recommended for externally managed systems.    

**JWT vs Reference token**

JWT token is a self-contained access token, whereas reference token is stored within IdentityServer storage and a unique identifier is issued back to the client as a reference for the token.

Logout is not supported with ROPG, but token revocation is supported but not for JWT. Hence, for this tutorial, we will be using Reference Tokens for web and mobile apps.

## Setup IdentityServer4

1.  Create a C# empty web api project    
2.  Add NuGet packages for    
	```csharp
	1. IdentityServer4
	2. IdentityServer4.AspNetIdentity 
	3. IdentityServer4.AspNetIdentity 
	```
        
## Setup Clients, Api resources and Scopes

1.  Create Config.cs and add resources    
    ```csharp
    public static IEnumerable<IdentityResource> GetIdentityResources()
    {
        return new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email()
        };
    }
    ```
    
2.  Create api scope
    
    ```csharp
    public static IEnumerable<ApiScope> GetApiScopes()
    {
        return new[]
        {
            new ApiScope(name: "myapi.access",   displayName: "Access API Backend")
        };
    }
    ```
    
3.  Add API resources with scopes. Add an api secret that will be used by the API to get the token from IdentityServer using the supplied reference token from their client app.
    
    ```csharp
    public static IEnumerable<ApiResource> GetApiResources()
    {
        return new List<ApiResource>
        {
            new ApiResource("myapi", "My API")
            {
                Scopes = new List<string>()
                {
                    "myapi.access"
                },
                ApiSecrets = { new Secret("secret".Sha256()) }
            }
        };
    }
    ```
    
4.  Create an API client with allowed scopes
    
    ```csharp
    public static IEnumerable<Client> GetClients()
    {
        return new List<Client>
        {
            new Client
            {
                ClientId = "reactWeb",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
                AllowedScopes = 
                { 
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "myapi.access" 
                },
                AllowOfflineAccess = true, // this to allow SPA,
                AlwaysSendClientClaims = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                // this will generate reference tokens instead of access tokens
                AccessTokenType = AccessTokenType.Reference 
            }
        };
    }
    ```
    
      
    NOTE: If you want to use JWT, use this instead  
    
    ```csharp
    AccessTokenType = AccessTokenType.Reference
    ```
    
5.  In Startup.cs, configure IdentityServer
    
    ```csharp
    services.AddIdentityServer(options =>
            {
                options.IssuerUri = issuer;
            })
            .AddDeveloperSigningCredential() // use a valid signing cert in production
            .AddInMemoryIdentityResources(Config.GetIdentityResources())    
            .AddInMemoryApiResources(Config.GetApiResources())
            .AddInMemoryApiScopes(Config.GetApiScopes())
            .AddInMemoryClients(Config.GetClients());
    ```
    
6.  In Startup.cs, set the COR policy. For this tutorial, we will allow anyone
    
    ```csharp
    services.AddSingleton<ICorsPolicyService>(serviceProvider =>
          new DefaultCorsPolicyService(serviceProvider.GetService<ILogger<DefaultCorsPolicyService>>())
          {
              AllowAll = true
          });
    ```
    
7.  In Startup.cs, configure the HTTP pipeline   

	```csharp
	app.UseIdentityServer();
	```

## Setup Custom User Storage with MongoDb

1.  Create ApplicationUser.cs and inherit from IdentityUser    
    ```csharp
    public class ApplicationUser : IdentityUser
    {
        //add your custom user properties here
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
    ```
    
2.  Create ApplicationRole and inherit from IdentityRole
    
    ```csharp
    public class ApplicationRole : IdentityRole, IEntity
    {
    }
    ```
    
3.  Create a generic MongoDb repository
    
    ```csharp
    public class MongoRepository<T> : IRepository<T>
            where T: class, new()
        {
            private readonly IMongoDatabase _database;
    
            public MongoRepository(IOptions<IdentitySection> options)
            {
                var optionsValue = options.Value;
                var client = new MongoClient(optionsValue.Mongo.ConnectionString);
                _database = client.GetDatabase(optionsValue.Mongo.IdentityServerDatabase);
            }
    
            private IMongoCollection<T> Collection() => _database.GetCollection<T>(typeof(T).Name.Camelize());
    
            public IQueryable<T> Where(Expression<Func<T, bool>> expression) => Collection().AsQueryable().Where(expression);
    
            public async Task Delete(Expression<Func<T, bool>> predicate) => await Collection().DeleteManyAsync(predicate);
    
            public async Task<T> Single(Expression<Func<T, bool>> expression)
            {
                var result = await Collection().FindAsync(expression);
    
                return result?.SingleOrDefault();
            }
    
            public async Task Update(T item, Expression<Func<T, bool>> expression) => await Collection().FindOneAndReplaceAsync(expression, item);
    
            public async Task Insert(T item) => await Collection().InsertOneAsync(item);
    
            public async Task Insert(IEnumerable<T> items) => await Collection().InsertManyAsync(items);
        }
    ```
    
4.  Create classes that implements the following:
    ```csharp
    1. IUserStore<ApplicationUser>
    2. IUserPasswordStore<ApplicationUser>
    3. IPasswordHasher<ApplicationUser>
    4. IRoleStore<ApplicationRole>
    ```
    ```csharp
    public class UserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>
    {
        private readonly IRepository<ApplicationUser> _repository;
        public UserStore(IRepository<ApplicationUser> repository)
        {
            _repository = repository;
        }
        
        public void Dispose()
        {
        }
    
        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
		    cancellationToken.ThrowIfCancellationRequested();
            if(string.IsNullOrWhiteSpace(user.UserName)) return IdentityResult.Failed();
            var result = await FindByNameAsync(user.UserName, CancellationToken.None);
            if (result != null) return IdentityResult.Failed();
            await _repository.Insert(user);
            return IdentityResult.Success;
        }
    
        public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
	       cancellationToken.ThrowIfCancellationRequested();
           var result = await FindByIdAsync(user.Id, CancellationToken.None);
           if (result == null) return IdentityResult.Failed();
           await _repository.Delete(u => u.Id == user.Id);
           return IdentityResult.Success;
        }
    
	    public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
	        cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(userId)) throw new RequiredArgumentException(nameof(userId));
            var result = await _repository.Single(u => u.Id == userId);
            return result;
        }
    
        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
	        cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(normalizedUserName)) throw new RequiredArgumentException(nameof(normalizedUserName));
            var result = await _repository.Single(u =>
            u.UserName.ToLowerInvariant() == normalizedUserName.ToLowerInvariant());
            return result;
         }
    
         public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
         {
	         cancellationToken.ThrowIfCancellationRequested();
             if (user == null) throw new RequiredArgumentException(nameof(user));
             var normalizedUsername = user.NormalizedUserName ?? user.UserName.ToUpperInvariant();
             return Task.FromResult(normalizedUsername);
         }
    
         public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
         {
	         cancellationToken.ThrowIfCancellationRequested();
             if (user == null) throw new RequiredArgumentException(nameof(user));
             return Task.FromResult(user.Id);
         }
    
         public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
         {
	         cancellationToken.ThrowIfCancellationRequested();
             if (user == null) throw new RequiredArgumentException(nameof(user));
             return Task.FromResult(user.UserName);
          }
    
          public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
          {
	          cancellationToken.ThrowIfCancellationRequested();
              if (user == null) throw new RequiredArgumentException(nameof(user));
              if (normalizedName == null) throw new RequiredArgumentException(nameof(normalizedName));
              user.NormalizedUserName = normalizedName;
              return Task.CompletedTask;
          }
    
          public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
          {
              cancellationToken.ThrowIfCancellationRequested();
              if (user == null) throw new RequiredArgumentException(nameof(user));
              if (userName == null) throw new RequiredArgumentException(nameof(userName));
              user.UserName = userName;
              return Task.CompletedTask;
          }
    
          public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
          {
              cancellationToken.ThrowIfCancellationRequested();
              if (user == null) return IdentityResult.Failed();
              var result = await FindByIdAsync(user.Id, CancellationToken.None);
              if (result == null) IdentityResult.Failed();
              await _repository.Update(user, u => u.Id == user.Id);
              return IdentityResult.Success;
          }
    
          public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
          {
              return Task.FromResult(user.PasswordHash);
          }
    
          public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
          {
              return Task.FromResult(string.IsNullOrWhiteSpace(user.PasswordHash));
          }
    
          public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
          {
              user.PasswordHash = passwordHash;
              return Task.CompletedTask;
          }
    }
    ```
    ```csharp
    public class UserPasswordHasher : PasswordHasher<ApplicationUser>
    {
    }
    ```   
    ```csharp
	public class RoleStore : IRoleStore<ApplicationRole>
	{
		private readonly IRepository<ApplicationRole> _repository;
		public RoleStore(IRepository<ApplicationRole> repository)
		{
			_repository = repository;
		}
	
		public void Dispose()
		{
		}

		public async Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var result = await FindByNameAsync(role.Name, CancellationToken.None);
			if (result != null) return IdentityResult.Failed();
			await _repository.Insert(role);
			return IdentityResult.Success;
		}

		public async Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var result = await FindByIdAsync(role.Id, CancellationToken.None);
			if (result == null) return IdentityResult.Failed();
			await _repository.Delete(r => r.Id == role.Id);
			return IdentityResult.Success;
		}

		public async Task<ApplicationRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (string.IsNullOrWhiteSpace(roleId)) throw new RequiredArgumentException(nameof(roleId));
			var result = await _repository.Single(r => r.Id == roleId);
			return result;
		}

		public async Task<ApplicationRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (string.IsNullOrWhiteSpace(normalizedRoleName)) throw new RequiredArgumentException(nameof(normalizedRoleName));
			var result = await _repository.Single(r => r.NormalizedName == normalizedRoleName);
			return result;
		}

		public Task<string> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (role == null) throw new RequiredArgumentException(nameof(role));
			return Task.FromResult(role.NormalizedName ?? role.Name.ToUpperInvariant());
		}

		public Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (role == null) throw new RequiredArgumentException(nameof(role));
			return Task.FromResult(role.Id);
		}

		public Task<string> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (role == null) throw new RequiredArgumentException(nameof(role));
			return Task.FromResult(role.Name);
		}

		public Task SetNormalizedRoleNameAsync(ApplicationRole role, string normalizedName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (role == null) throw new RequiredArgumentException(nameof(role));
			if (string.IsNullOrWhiteSpace(normalizedName)) throw new RequiredArgumentException(nameof(normalizedName));
			role.NormalizedName = normalizedName;
			return Task.CompletedTask;
		}

		public Task SetRoleNameAsync(ApplicationRole role, string roleName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (role == null) throw new RequiredArgumentException(nameof(role));
			if (string.IsNullOrWhiteSpace(roleName)) throw new RequiredArgumentException(nameof(roleName));
			role.Name = roleName;
			return Task.CompletedTask;
		}

		public async Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (role == null) return IdentityResult.Failed();
			var result = await FindByIdAsync(role.Id, CancellationToken.None);
			if (result == null) IdentityResult.Failed();
			await _repository.Update(role, r => r.Id == role.Id);
			return IdentityResult.Success;
		}
	}
    ```    
        
5.  In Startup.cs, register services:
    
    ```csharp
    services.AddIdentity<ApplicationUser, ApplicationRole>().AddDefaultTokenProviders();
    services.AddTransient<IUserStore<ApplicationUser>, UserStore>();
    services.AddTransient<IUserPasswordStore<ApplicationUser>, UserStore>();
    services.AddTransient<IPasswordHasher<ApplicationUser>, UserPasswordHasher>();
    services.AddTransient<IRoleStore<ApplicationRole>, RoleStore>();
    ```
    
6.  In Startup.cs, register Identity User
    
    ```csharp
    services.AddIdentityServer(options =>
    {
        options.IssuerUri = issuer; //url of your identity server
    })
    // codes removed for brevity
    .AddAspNetIdentity<ApplicationUser>();
    ```
    

## Setup custom profile service

This service will let developers add custom claims to the identity token

1.  Create ProfileService and inherit from IProfileService
    
    ```csharp
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
    
        public ProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
    
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await GetUser(context.Subject);
            context.IssuedClaims = GetClaims(user).ToList();
        }
    
        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await GetUser(context.Subject);
            context.IsActive = user?.IsActive ?? false;
        }
    
        private async Task<ApplicationUser> GetUser(IPrincipal principal)
        {
            var userId = principal.GetSubjectId();
            return await _userManager.FindByIdAsync(userId);
        }
    
        private static IEnumerable<Claim> GetClaims(ApplicationUser user)
        {
            return new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, user.Id),
                new Claim("firstname", user.Firstname),
                new Claim("lastname", user.Lastname)
            };
        }
    }
    
    ```
          
    NOTE: The injected UserManager<ApplicationUser> will use the custom user storage we have created from previous steps
    
2.  In Startup.cs, register the profile service
    
    ```csharp
    services.AddTransient<IProfileService, ProfileService>();
    
    services.AddIdentityServer(options =>
    {
        options.IssuerUri = issuer; //url of your identity server
    })
    // codes removed for brevity
    .AddAspNetIdentity<ApplicationUser>()
    .AddProfileService<ProfileService>();
    ```
    

Make sure ```AddProfileService<>``` comes after ```AddAspNetIdentity<>``` to override some service registration, otherwise your custom profile service won’t work.

## Setup IdentityServer with Redis cache

1.  Install NuGet package for IdentityServer4.Contribe.RedisStore
    
2.  Set connection string for redis on appsettings.json
    
    ```csharp
    "ConnectionStrings": {
      "Redis": "localhost"
    }
    ```
    
3.  In Startup.cs, setup operational and caching store
    
    ```csharp
    services.AddIdentityServer(options =>
    {
        options.IssuerUri = issuer; //url of your identity server
    })
    // codes removed for brevity
    .AddOperationalStore(options =>
    {
        options.RedisConnectionString = redisConnection;
        options.Db = 1;
    })
    .AddRedisCaching(options =>
    {
        options.RedisConnectionString = redisConnection;
        options.KeyPrefix = "prefix";
    });
    ```
    

## Setup API resource

This is an API project that will use our newly created IdentityServer as our Authentication Server. API consumers (i.e. SPA or mobile) will request an access token to the IdentityServer for this API resource, then pass the generated token to an endpoint of the API application. The API will then call IdentityServer and ask to validate the token, and if valid should allow their request and return a valid response.

1.  Create an empty C# API project
    
2.  Install the following NuGet packages:
	```csharp
	1. Microsoft.AspNetCore.Authentication.JwtBearer
	2. IdentityModel.AspNetCore.OAuth2Introspection
	3. IdentityModel.AspNetCore
	```            
3.  In Startup.cs → ConfigureService, register controllers
    
    ```csharp
    services.AddControllers()
    //this is optional..
    //will use Enum keys instead of values on json responses
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.IgnoreNullValues = true;
    });
    ```
    
4.  In Startup.cs → ConfigureService, add Authentication for JWT token and set it as the default
    
    ```csharp
     services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
          .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
          {
              //url of your identityserver
              options.Authority = authority; 
              //value of the api resource setup from identityserver
              options.Audience = "myapi"; 
              //set to true if you require https for your identityserver
              options.RequireHttpsMetadata = false; 
              // if token does not contain a dot, it is a reference token
              // if it's a reference token, will forward it to introspection
              options.ForwardDefaultSelector = Selector.ForwardReferenceToken(apiGatewayConfiguration.Identity.Scheme.Introspection);
		    })
    ```
    
    NOTE: If the token passed is a reference token, the default Authentication will forward it to introspection. See below
    
5.  In Startup.cs → ConfigureService, add Authentication for reference token (introspection)
    
    ```csharp
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            //code removed for brevity
            
        }).AddOAuth2Introspection("introspection", options =>
        {
            //url of your identityserver
            options.Authority = authority; 
            //value of the api resource from identityserver
            options.ClientId = "myapi"; 
            //value of the api resource secret from identityserver
            options.ClientSecret = "secret"; 
            options.DiscoveryPolicy = new DiscoveryPolicy
            {
                //set to true if you require https for your identityserver
                RequireHttps = false 
            };
        });
    ```
    
6.  In Startup.cs → ConfigureService, set the CORS policy
    
    ```csharp
    //configure CORS policy for production. this is only for development
    services.AddCors(o => o.AddPolicy("LocalPolicy", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    }));
    ```
    
7.  In Startup.cs → Configure, apply the CORS policy
    
    ```csharp
    if (env.IsDevelopment())
    {
        ...//code removed for brevity
        app.UseCors("LocalPolicy");
    }
    ```
    
8.  In Startup.cs → Configure, use Authentication and Authorization
    
    ```csharp
    app.UseAuthentication();
    app.UseAuthorization();
    ```
    
9.  On your controller class, decorate it with Authorize attribute    
	```csharp
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class MyAuthenticatedController : ControllerBase
	{
		public MyAuthenticatedController()
		{
			//access the authenticated user and its claims
			var user = this.User;
		}
	}
	```
        

## Setup Signing Certificate

IdentityServer needs an assymetric key pair - a public and private keys for encrypting / decrypting messages - to sign or validate tokens.

On a cluster environment, all instances of the IdentityServer should use the same signing certificate. (i.e. store on a secure location or shared s3 bucket). For signing rotation - where an expired certificate has to be replaced by a new one - follow the steps [here](https://brockallen.com/2019/08/09/identityserver-and-signing-key-rotation/).

1.  Create a shell script generator.sh and save it somewhere
    
    ```bash
    #!/bin/bash
    
    DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null && pwd )"
    mkdir $DIR/values
    PRIVATE_PEM=$DIR/values/private.pem
    PUBLIC_PEM=$DIR/values/public.pem
    PFX=$DIR/values/mycert.pfx
    PASSWD=$1
    
    if [ -z "$PASSWD" ]
    then
        PASSWD="hardtoguess"
    fi
    
    echo "Creating Private Key"
    openssl genrsa 4096 > $PRIVATE_PEM
    
    echo "Creating Public Key"
    echo """AU
    NSW
    Sydney
    bizzposolutions
    D1C3
    d1c3@bizzposolutions.com.au
    """ | openssl req -x509 -days 1825 -new -key $PRIVATE_PEM -out $PUBLIC_PEM
    
    echo ""
    echo "Creating Certificate"
    
    openssl pkcs12 -export -in $PUBLIC_PEM -inkey $PRIVATE_PEM -out $PFX -password pass:$PASSWD
    ```
    
2.  Execute the script and it should create a folder called values with mycert.pfx
    
3.  Copy the generated file mycert.pfx and store it somewhere safe. For this tutorial, copy it on the root directory of the api project.
    
4.  In the api project → , load the signing certificate and update the registered signing credential
    
    ```csharp
    var cert = new X509Certificate2("mycert.pfx", "hardtoguess");
    
    //FROM 
    services.AddIdentityServer(...)
         .AddDeveloperSigningCredential()
    
    //CHANGE TO
    services.AddIdentityServer(...)
         .AddSigningCredential(cert)
    ```
    

## Seed users (for testing)

To create a user, we need to use the injected services for:
```csharp
* IPasswordHasher<ApplicationUser>
* IUserStore<ApplicationUser>
```

1.  Create an interface for ISeed
    
    ```csharp
    public interface ISeeder
    {
        Task EnsureSeed(CancellationToken cancellationToken = default);
    }
    ```
    
2.  Create a class to seed users and inherit from ISeeder
    
    ```csharp
    public class SeedUsers : ISeeder
    {
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly ILogger<SeedUsers> _logger;
    
        public SeedUsers(
            IUserStore<ApplicationUser> userStore,
            IPasswordHasher<ApplicationUser> passwordHasher,
            ILogger<SeedUsers> logger)
        {
            _userStore = userStore;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }
    
        public async Task EnsureSeed(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Ensuring User Seed Data");
    
            foreach (var user in SeedData)
            {
                var exists = await Exists(user.UserName, cancellationToken);
                if(exists)
                    _logger.LogDebug("User {UserName} already exist.", user.UserName);
                else
                {
                    var result = await _userStore.CreateAsync(user, cancellationToken);
                    if (!result.Succeeded)
                        _logger.LogError("Can not create seed user: {SeedErrors}", string.Join(",", result.Errors.Select(r => $"{r.Code}: {r.Description}")));
                    else
                        _logger.LogInformation("Seed user {UserName} created successfully.", user);
                }
            }
        }
    
        private async Task<bool> Exists(string username, CancellationToken cancellationToken)
        {
            var result = await _userStore.FindByNameAsync(username, cancellationToken);
            return result != null;
        }
    
        private IList<ApplicationUser> SeedData => new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "dev",
                PasswordHash = _passwordHasher.HashPassword(null, "secret"),
                Firstname = "dev",
                Lastname = "eloper",
                IsActive = true
            }
        };
    }
    ```
    
3.  Create a class extension for IServiceProvider to execute seeders
    
    ```csharp
    public static async Task Seed(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
    
        var seeders = scope
            .ServiceProvider
            .GetServices<ISeeder>()
            .ToList();
    
        foreach (var seeder in seeders)
        {
            await seeder.EnsureSeed(cancellationToken);
        }
    }
    ```
    
4.  In Program.cs, update the entry point to execute the seeders
    
    ```csharp
    public static Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        await host.Services.Seed();
        await host.RunAsync();
    }
    ```
    

## IdentityServer Endpoints

-   Request token    
    -   url: {{auth_host}}/connect/token
    -   method: POST
    -   header:
        -   Authorization: Bearer Token
    -   body (x-www-form-urlencoded) :
        -   grant_type = “password” 
        -   username = “dev”
        -   password = “secret”            
        -   scope = “myapi.access offline_access openid”            
        -   client_id = “reactWeb”            
        -   client_secret = “secret”
            
    -   response:        
        -   access_token = this is your reference token            
        -   refresh_token = this is your refresh token. will be used to request new reference token once it expires. expires in 30 days.            
    
-   Revoke token    
    -   url: {{auth_host}}/connect/revocation        
    -   method: POST        
    -   header:        
        -   Authorization: Basic Auth            
            -   user: “reactWeb”                
            -   password: “secret”                
    -   body (x-www-form-urlencoded) :        
        -   token: {{value of your reference token}}            
        -   token_type_hint: “access_token”
                    
-   Refresh token    
    -   url: {{auth_host}}/connect/token        
    -   method: POST        
    -   body (x-www-form-urlencoded):        
        -   grant_type = “refresh_token”            
        -   client_id = “reactWeb”            
        -   client_secret = “secret”            
        -   refresh_token = {{value of your refresh token as per response from get token}}           
    -   returns        
        -   access_token = your new reference token            
        -   refresh_token = your new refresh token
            
-   Get User Info    
    -   url: {{auth_host}}/connect/userinfo        
    -   method: GET        
    -   header:        
        -   Authorization: Bearer Token            
    -   returns:        
        -   user info and claims (from what was setup in ProfileService)
