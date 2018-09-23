using Achievements.Hubs;
using Achievements.Models;
using Achievements.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.Swagger.Model;
using System.IO;

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
            var connectionString = Configuration.GetConnectionString("Database");

            services.AddTransient<IRepository<Achievement, int>, AchievementsRepository>(
                serviceProvider => new AchievementsRepository(connectionString));
            services.AddTransient<IUserAchievementsRepository<string>, UserAchievementsRepository>(
                serviceProvider => new UserAchievementsRepository(connectionString));

            services.AddCors();

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddSignalR().AddMessagePackProtocol();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var app = PlatformServices.Default.Application;
            var xmlPath = Path.Combine(app.ApplicationBasePath, "Achievements.xml");
            services.AddSwaggerGen();
            services.ConfigureSwaggerGen(options =>
            {
                options.SingleApiVersion(new Info
                {
                    Version = "v1",
                    Title = "Achievements",
                    TermsOfService = "None",
                    Contact = new Contact { Name = "Achievements", Email = "rob@gcsuk.co.uk", Url = "www.gcsuk.co.uk" }
                });
                options.IncludeXmlComments(xmlPath);
                options.DescribeAllEnumsAsStrings();
                options.CustomSchemaIds(x => x.FullName);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var origins = new[]
            {
                "https://nexussite.z35.web.core.windows.net",
                "https://www.nexusbet.co.uk",
                "https://nexusbet.co.uk",
                "https://nexusbet-staging.azurewebsites.net",
                "http://localhost:54321",
                "http://localhost:3000"
            };

            app.UseCors(builder => builder.WithOrigins(origins).AllowAnyMethod().AllowAnyHeader().AllowCredentials());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSignalR(routes => routes.MapHub<AchievementsHub>("/achievements"));

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUi();
        }
    }
}
