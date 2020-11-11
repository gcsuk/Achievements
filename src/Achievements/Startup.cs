using Achievements.Demo;
using Achievements.Events;
using Achievements.Hubs;
using Achievements.Models.Entities;
using Achievements.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.ServiceBus;
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
            services.AddCors();

            services.AddSingleton<IUserIdProvider, QueryStringUserIdProvider>();
            services.AddSingleton<IQueueClient, QueueClient>(
                serviceProvider => new QueueClient(Configuration.GetConnectionString("ServiceBus"), "unlockedachievements"));
            services.AddSingleton<IEventSender, EventSender>();
            services.AddSingleton<IRepository<AchievementEntity>, AchievementsRepository>();
            services.AddSingleton<IRepository<UnlockedAchievementEntity>, UnlockedAchievementsRepository>();

            services.AddHostedService<EventListener>();

            services.AddSignalR();
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
