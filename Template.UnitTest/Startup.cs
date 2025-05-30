using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Template.Domain.AppSetting;
using Template.Helper;
using Template.Infrastructure;
using Template.Service.IServices;
using Template.Service.Services;

namespace Template.UnitTest
{
    public class Startup
    {
        private IConfiguration _configuration;
        public void ConfigureHost(IHostBuilder hostBuilder) =>
        hostBuilder
            .ConfigureHostConfiguration(builder => { })
            .ConfigureAppConfiguration((context, builder) => {
                builder.AddJsonFile("appsettings.json");
                _configuration = builder.Build();
            });

        public void ConfigureServices(IServiceCollection services)
        {
            //Mapping appsetting.json to class
            services.Configure<JWTData>(_configuration.GetSection("JWT"));
            services.Configure<CustomSettingData>(_configuration.GetSection("CustomSetting"));

            //Add DbContext
            var dbConnectionStringData = services.BuildServiceProvider().GetRequiredService<IOptions<CustomSettingData>>().Value;

            if (dbConnectionStringData != null)
            {
                string dbConnectionString = dbConnectionStringData.DbConnectionString ?? "";

                services.AddDbContext<TemplateDbContext>(options =>
                {
                    options.UseSqlServer(dbConnectionString);
                });
            }
        
            //Add Service
            services.AddScoped<IErrorExceptionHandler, ErrorExceptionHandler>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
        }
    }
}

