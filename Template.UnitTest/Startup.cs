using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Template.Domain.AppSetting;
using Template.Helper.DataProtected;
using Template.Helper.ErrorException;
using Template.Helper.PasswordHash;
using Template.Helper.Token;
using Template.Infrastructure;
using Template.Service.IServices;
using Template.Service.Services;

namespace Template.UnitTest
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var baseDirectory = AppContext.BaseDirectory;

            var _configuration = new ConfigurationBuilder().SetBasePath(baseDirectory).AddJsonFile("appsettings.json", false, true).Build();

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

            //Add DataProtection 
            var dataProtectionPath = Path.Combine(baseDirectory, "dataProtectedInformation");

            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath)).SetApplicationName("Template").SetDefaultKeyLifetime(TimeSpan.FromDays(90));

            //Add Service
            services.AddScoped<IErrorExceptionHandler, ErrorExceptionHandler>();
            services.AddScoped<IDataProtected, DataProtected>();
            services.AddScoped<IPasswordHash, PasswordHash>();
            services.AddScoped<IToken, Token>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IDataProtected, DataProtected>();
        }
    }
}

