# Auth server with user interaction (OpenId)
### Features
* Login / Logout
* Registration with confirmation email
* Forgot password with reset password link
* Customizable email templates using Handlebars.Net

#### A. Setup server
1. Create an empty .Net MVC app
2. Install NuGet package `BizzPo.IdentityServer`
   ```bash
   dotnet add package BizzPo.IdentityServer
   ```
3. Install NuGet package `BizzPo.IdentityServer.Hosts.Server`
   ```bash
   dotnet add package BizzPo.IdentityServer.Hosts.Server
   ```
4. In `Startup.cs`, configure services for Identity User Interaction
   ```csharp
   services.AddControllersWithViews()
       .AddIdentityServerUserInteraction();
    ```
5. Configure scope to for user interaction
   ```csharp
   services.AddControllersWithViews()
       .AddIdentityServerUserInteraction(config => {
           config.Scope = "users.management";
       });
   ```
6. Configure email templates
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
7. Configure for the built-in user interaction mvc controllers (skip this step if you want to create your own controllers)
   ```csharp
   services.AddControllersWithViews()
       .AddIdentityServerUserInteraction(config =>
       {
           //codes removed for brevity
       })
       .AddIdentityServerUserInteractionMvc();
   ```
8. Configure OpenId
   ```csharp
   services.AddAuthentication(options =>  
       {  
           options.DefaultScheme = "Cookies";  
           options.DefaultChallengeScheme = "oidc";  
       })  
       .AddIdentityServerUserAuthorization();
   ```
9. Configure IdentityServer4 with MongoDb and Redis cache
   ```csharp
   services.AddIdentityServerMongoDb()  
       .AddRedisCache()  
       .AddDeveloperSigningCredential()  
       .AddIdentityServerUserAuthorization<ApplicationUser, ApplicationRole>()  
   ``` 
10. Within the root directory of the project, execute this script to download the default views
    ```bash
    curl -L https://raw.githubusercontent.com/markglibres/identityserver4-mongodb-redis/master/utils/download_sample_views.sh | bash
    ```
#### B. Setup client app
1. Create an empty .Net MVC app
2. Install NuGet package `BizzPo.IdentityServer.User.Client`
   ```bash
   dotnet add package BizzPo.IdentityServer.User.Client
   ```
