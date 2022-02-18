using AutoMapper;
using Turing.Api.Configuration;
using Turing.Api.Filters;
using Turing.Api.ModelBinders;
using Turing.Business.Identity;
using Turing.Business.Services;
using Turing.Core.Configuration;
using Turing.Core.Identity;
using Turing.Core.Services;
using Turing.Data.EntityFramework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Turing.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext(Configuration.GetConnectionString("DbConnectionString"));
            services.AddAutoMapper();
            services.AddSwagger();
            services.AddJwtIdentity(Configuration.GetSection(nameof(JwtConfiguration)));

            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<IProfilesService, ProfilesService>();
            services.AddTransient<IJwtFactory, JwtFactory>();

            services.AddMvc(options =>
            {
                options.ModelBinderProviders.Insert(0, new OptionModelBinderProvider());
                options.Filters.Add<ExceptionFilter>();
                options.Filters.Add<ModelStateFilter>();
            })
            .AddJsonOptions();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ApplicationDbContext dbContext)
        {
            loggerFactory.AddLogging(Configuration.GetSection("Logging"));

            if (env.IsDevelopment())
            {
                dbContext.Database.EnsureCreated();
            }

            app.UseSwagger("My Web API.");
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
