using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Identity.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(builder => 
                {
                    builder ??= new ConfigurationBuilder();
                    var appEnv = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
                    if(string.IsNullOrWhiteSpace(appEnv))
                        appEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                    builder
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", false, true)
                        .AddJsonFile($"appsettings.{appEnv}.json", true, true)
                        .AddEnvironmentVariables();

                    if (args != null) builder.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<Startup>();
                    var serviceProvider = services.BuildServiceProvider();
                    
                    var startup = serviceProvider.GetRequiredService<Startup>();
                    startup.ConfigureServices(services);
                });
    }
}