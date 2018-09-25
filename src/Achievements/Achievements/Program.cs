using Achievements.Events;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Achievements
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .AddEnvironmentVariables()
            .Build();

        public static void Main(string[] args)
        {
            EventListener.InitialiseQueue(Configuration.GetConnectionString("ServiceBus"), Configuration.GetConnectionString("Database"));

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(Configuration)
                .UseStartup<Startup>();
    }
}
