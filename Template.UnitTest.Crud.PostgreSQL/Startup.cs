using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Template.Domain.AppSetting;
using Template.Helper.PasswordHash;
using Template.Infrastructure.PostgreSQL;

namespace Template.UnitTest.Crud.PostgreSQL
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var baseDirectory = AppContext.BaseDirectory;

            var _configuration = new ConfigurationBuilder().SetBasePath(baseDirectory).AddJsonFile("appsettings.json", false, true).Build();

            //Mapping appsetting.json to class
            services.Configure<CustomSettingData>(_configuration.GetSection("CustomSetting"));

            //Add Service
            services.AddScoped<IPasswordHash, PasswordHash>();

            //Add DataProtection 
            var dataProtectionPath = Path.Combine(baseDirectory, "dataProtectedInformation");
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath)).SetApplicationName("Template").SetDefaultKeyLifetime(TimeSpan.FromDays(90));
  
            //Add DbContext PostgreSQL Server
            services.AddDbContext<TemplateDbContext>(options =>
            {
                var customSettingData = services.BuildServiceProvider().GetRequiredService<IOptions<CustomSettingData>>().Value;

                if (customSettingData != null)
                {
                    var connectionPostgreSQLServer = customSettingData.ConnectionPostgreSQLServer ?? "";

                    options.UseNpgsql(connectionPostgreSQLServer, s =>
                    {
                        s.MigrationsAssembly("Template.Infrastructure.PostgreSQL");
                    });
                }
                else
                {
                    throw new BadHttpRequestException("custom Setting Error.");
                }
            });
        }
    }
}

