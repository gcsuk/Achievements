using Achievements.Demo;
using Achievements.Events;
using Achievements.Hubs;
using Achievements.Models;
using Achievements.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Achievements
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR().AddMessagePackProtocol();

            var connectionString = Configuration.GetConnectionString("Database");

            services.AddSingleton<IRepository<Achievement, int>, AchievementsRepository>(
                serviceProvider => new AchievementsRepository(connectionString));
            services.AddSingleton<IUserAchievementsRepository<string>, UserAchievementsRepository>(
                serviceProvider => new UserAchievementsRepository(connectionString));

            services.AddCors();

            services.AddSingleton<EventSender>();

            services.AddHostedService<EventListener>();

            services.AddSingleton<IUserIdProvider, QueryStringUserIdProvider>();

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Achievements" , Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<AchievementsHub>("/achievementshub");
            });

            app.UseSwagger();
            app.UseSwaggerUI((c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Achievements V1");
            }));
        }
    }
}
