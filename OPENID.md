# Auth server with user interaction (OpenId)
### Features
* Login / Logout
* Registration with confirmation email
* Forgot password with reset password link
* Customizable email templates using Handlebars.Net

#### A. Setup server
1. Create an empty .Net MVC app
2. Within the root directory of the project, execute this script to download the sample views, templates, users and docker compose file (for mongodb, redis and mailhog)
   ```bash
   curl -L https://raw.githubusercontent.com/markglibres/identityserver4-mongodb-redis/master/utils/download_sample.sh | bash
   ```
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
7. Configure email templates
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
11. Seed clients and scopes
    ```csharp
    services.AddIdentityServerMongoDb()  
        // codes removed for brevity
        .SeedClients<IdentityServerClients>()
        .SeedApiResources<UsersApiResource>()
        .SeedApiScope<UsersApiScopes>()
        .SeedIdentityResource<SeedIdentityResources>();
    ``` 
12. In `Startup.cs -> Configure`, configure request pipeline for IdentityServer.
    ```csharp
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)  
    {
        //codes removed for brevity
        app.UseIdentityServer();  
        app.UseAuthorization();
        //codes removed for brevity
    }
    ```
13. In appsettings.json,  configure mongodb and redis connection string and smtp options
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
14. Run docker-compose file for mongodb, redis and mailhog (smtp server)
    ```bash
    docker-compose -f docker-compose-db.yaml up -d 
    ```
    endpoints:
   * Auth server: https://localhost:5001/
   * Mailhog: http://localhost:8025/
   * MongoDb: localhost:27017
   * Redis: localhost:6379

15. Run the auth server ```dotnet run```. Homepage should show, then browse to the discovery endpoint: https://localhost:5001/.well-known/openid-configuration

#### B. Setup client app
1. Create an empty .Net MVC app
2. Install NuGet package `BizzPo.IdentityServer.User.Client`
   ```bash
   dotnet add package BizzPo.IdentityServer.User.Client
   ```
