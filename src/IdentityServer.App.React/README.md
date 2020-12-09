How to configure MVC with identityserver

1. Configure for user management api endpoints
    services
        .AddControllersWithViews()
        .AddIdentityServerUserApi();

2. services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddIdentityServerUserAuthentication();

3. services
                   .AddIdentityServerMongoDb()
                   .AddRedisCache()
                   .AddDeveloperSigningCredential()
                   .AddIdentityServerUser<ApplicationUser, ApplicationRole>()
                   .SeedUsers<ApplicationUser, SeedUsers<ApplicationUser>>()
                   .SeedClients<ApiClients>()
                   .SeedClients<IdentityServerClients>()
                   .SeedApiResources<UsersApiResource>()
                   .SeedApiScope<UsersApiScopes>()
                   .SeedIdentityResource<SeedIdentityResources>();


4. app.UseIdentityServer();
               app.UseAuthentication();
               app.UseAuthorization();
