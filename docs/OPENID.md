# Auth server with user interaction (OpenId)
### Features
* Login / Logout
* Registration with confirmation email
* Forgot password with reset password link
* Customizable email templates using Handlebars.Net
### Pre-requisites
* docker (linux)
* docker-compose
* dotnet 3.x (upcoming support for 5.x)
* use https for server and client apps

#### A. Setup server
1. Create an empty .Net MVC app.
    * use the url https://localhost:5001 for this tutorial, otherwise you need to update the config values in next steps
2. Within the root directory of the project, execute this script to download the sample views, templates, sample seed users / clients / api resources, and docker compose file (for mongodb, redis and mailhog)
   ```bash
   curl -L https://raw.githubusercontent.com/markglibres/identityserver4-mongodb-redis/master/utils/server_sample.sh | bash
   ```
   *-----skip this step if you don't want to use the sample files*
3. Install NuGet package `BizzPo.IdentityServer`
   ```bash
   dotnet add package BizzPo.IdentityServer
   ```
4. Install NuGet package `BizzPo.IdentityServer.Hosts.Server`
   ```bash
   dotnet add package BizzPo.IdentityServer.Hosts.Server
   ```
5. In `Startup.cs -> ConfigureServices`, configure services for Identity User Interaction
   ```csharp
   services.AddControllersWithViews()
       .AddIdentityServerUserInteraction();
    ```
6. Configure scope to use for user interaction
   ```csharp
   services.AddControllersWithViews()
       .AddIdentityServerUserInteraction(config => {
           config.Scope = "users.management";
       });
   ```
7. Configure email templates - make sure to set "Content -> Copy always / newer" or "Embedded" for email templates
   ```csharp
   services.AddControllersWithViews()
       .AddIdentityServerUserInteraction(config => {
           //.....
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
                       File = "~/Templates/user-reset-password.html",
                       FileStorageType = FileStorageTypes.Local
                   }
               }
           };
       });
   ```
8. Configure for the built-in user interaction mvc controllers `.AddIdentityServerUserInteractionMvc()`

   (skip this step if you want to create your own controllers.. TODO: doc on how to send command / query from custom controllers)

   ```csharp
   services.AddControllersWithViews()
       .AddIdentityServerUserInteraction(config =>
       {
           //codes removed for brevity
       })
       .AddIdentityServerUserInteractionMvc();
   ```

9. Configure OpenId
   ```csharp
   services.AddAuthentication(options =>  
       {  
           options.DefaultScheme = "Cookies";  
           options.DefaultChallengeScheme = "oidc";  
       });
   ```
10. Configure IdentityServer4 with MongoDb and Redis cache
    ```csharp
    services.AddIdentityServerMongoDb()  
        .AddRedisCache()  
        .AddDeveloperSigningCredential()  
        .AddIdentityServerUserAuthorization<ApplicationUser, ApplicationRole>()  
    ``` 
11. Seed clients, scopes and users
    ```csharp
    services.AddIdentityServerMongoDb()  
        // codes removed for brevity
        .AddIdentityServerUserAuthorization<ApplicationUser, ApplicationRole>()  
        .SeedUsers<ApplicationUser, SeedUser>()
        .SeedClients<IdentityServerClients>()
        .SeedApiResources<UsersApiResource>()
        .SeedApiScope<UsersApiScopes>()
        .SeedIdentityResource<SeedIdentityResources>();
    ``` 
12. In `Program.cs -> Main`, initialize the seeds
    ```csharp
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();  
        await host.Services.Initialize();  
        await host.Services.Initialize<ApplicationUser>();  
        await host.RunAsync();
    }
    ```
13. In `Startup.cs -> Configure`, configure request pipeline for IdentityServer.
    ```csharp
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)  
    {
        //codes removed for brevity
        app.UseIdentityServer();  
        app.UseAuthorization();
        //codes removed for brevity
    }
    ```
14. In appsettings.json,  configure mongodb and redis connection string and smtp options
    ```javascript
    "Identity": {  
      "Server":  {  
        "Authority": "https://localhost:5001",  //host url of your identityserver
        "RequireConfirmedEmail": true, //if user registration requires confirmation of email 
        "Mongo": {  
            //connectionstring for your mongodb
          "ConnectionString": "mongodb://root:foobar@localhost:27017/?readPreference=primaryPreferred&appname=identityserver",  
          "Database": "Identity"  //database name for identityserver
        },  
        "Redis": {  
          "ConnectionString": "localhost",  // connection string for redis
          "Db": -1,  
          "Prefix": "test"  
        }  
      }  
    },
    "Smtp": {
        "Host": "localhost",
        "Port": 1025,
        "Username": "",
        "Password": "",
        "SenderEmail": "system.user@bizzpo.com",
        "SenderName": "BizzPo IdentityServer",
        "IsSandBox": false
    }
    ```
15. Run docker-compose file for mongodb, redis and mailhog (smtp server)
    ```bash
    docker-compose -f docker-compose-db.yaml up -d 
    ```
    endpoints:
    * Auth server: https://localhost:5001/
    * Mailhog: http://localhost:8025/
    * MongoDb: localhost:27017
    * Redis: localhost:6379

16. Run the auth server ```dotnet run```. Homepage should show, then browse to the discovery endpoint: https://localhost:5001/.well-known/openid-configuration

Your authorization server is ready, however you need to setup your client app to complete the authorization code flow.

#### B. Setup client app
1. Create an empty .Net MVC app
    - use the url https://localhost:5002 for this tutorial, otherwise you need to update the identity client from previous steps

   Within the root directory of the project, execute the script below to download the sample views and controllers
   ```bash
   curl -L https://raw.githubusercontent.com/markglibres/identityserver4-mongodb-redis/master/utils/client_sample.sh | bash
   ```

   *-----skip this step if you don't want to use the sample files*

2. Install NuGet package `BizzPo.IdentityServer.User.Client`
   ```bash
   dotnet add package BizzPo.IdentityServer.User.Client
   ```

3. In `Startup.cs -> ConfigureService`, configure default authentication schema to OpenId
   ```csharp
   services.AddAuthentication(options =>  
       {  
           options.DefaultScheme = "Cookies";  
           options.DefaultChallengeScheme = "oidc";  
       })
   ```

4. Configure cookie authentication
   ```csharp
   services.AddAuthentication(options =>  
           {  
               options.DefaultScheme = "Cookies";  
               options.DefaultChallengeScheme = "oidc";  
           })
           .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
   ```

5. Configure OpenId connect
   ```csharp
   services
       // codes removed for brevity
       .AddOpenIdConnect("oidc", options =>  
       {  
           //the auth server url we configured on previous steps
           options.Authority = "https://localhost:5001";  
           // the seeded client from previous steps
           options.ClientId = "mvc";  
           options.ClientSecret = "secret";  
           options.ResponseType = "code";  
           options.SaveTokens = true;  
           options.GetClaimsFromUserInfoEndpoint = true;  
       })
   ```

6. Configure to use the user interaction endpoints
   ```csharp
   services
       // codes removed for brevity
       .AddUserManagement(options =>  
       {  
           options.AuthenticationScheme = "oidc";  
           // the user-interaction scope we defined in auth server from previous steps
           options.Scope = "users.management";  
       });
   ```

7. In `Startup.cs -> Configure`, set to use authentication
   ```csharp
   app.UseRouting();  
   app.UseAuthentication();  
   app.UseAuthorization();
   ```

8. Don't forget to set the CORS policy to allow redirection from your auth server
   ```csharp
   // Startup.cs -> ConfigureService
   services.AddCors(options =>  
   {  
       options.AddPolicy("All",  
           builder =>  
           {  
               builder  
                   .WithOrigins("http://localhost:5001")  
                   .AllowCredentials()  
                   .AllowAnyHeader()  
                   .AllowAnyMethod();  
           });  
   });
   
   // Startup.cs -> Configure
   app.UseCors("All");
   ```
9. Run the project `dotnet core`
