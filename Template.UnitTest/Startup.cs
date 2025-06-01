using Hangfire;
using Hangfire.Redis.StackExchange;
using MassTransit;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Template.Domain.AppSetting;
using Template.Helper.DataProtected;
using Template.Helper.ErrorException;
using Template.Helper.MessageConsume;
using Template.Helper.MessagePublish;
using Template.Helper.PasswordHash;
using Template.Helper.Redis;
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
            services.Configure<RabbitMQData>(_configuration.GetSection("RabbitMQ"));
            services.Configure<RedisData>(_configuration.GetSection("Redis"));
            services.Configure<HangfireData>(_configuration.GetSection("Hangfire"));

            //Add DbContext
            services.AddDbContext<TemplateDbContext>(options =>
            {
                var customSettingData = services.BuildServiceProvider().GetRequiredService<IOptions<CustomSettingData>>().Value;

                if (customSettingData != null)
                {
                    var dbConnectionStringData = customSettingData.DbConnectionString ?? "";

                    options.UseSqlServer(dbConnectionStringData, s =>
                    {
                        s.MigrationsAssembly("Template.Infrastructure");
                    });
                }
                else
                {
                    throw new BadHttpRequestException("custom Setting Error.");
                }
            });

            //Add DataProtection 
            var dataProtectionPath = Path.Combine(baseDirectory, "dataProtectedInformation");

            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath)).SetApplicationName("Template").SetDefaultKeyLifetime(TimeSpan.FromDays(90));

            //Add RabbitMQ
            services.AddMassTransit(options =>
            {
                var rabbitMQData = services.BuildServiceProvider().GetRequiredService<IOptions<RabbitMQData>>().Value;

                if (rabbitMQData != null)
                {
                    string hostName = rabbitMQData.HostName ?? "";
                    string username = rabbitMQData.UserName ?? "";
                    string password = rabbitMQData.Password ?? "";
                    string queueName = rabbitMQData.QueueName ?? "";

                    options.AddConsumer<MessageConsume>();
                    options.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host(hostName, "/", h =>
                        {
                            h.Username(username);
                            h.Password(password);
                        });
                        cfg.ReceiveEndpoint(queueName, r =>
                        {
                            r.ConfigureConsumer<MessageConsume>(context);
                        });
                    });
                }
                else
                {
                    throw new BadHttpRequestException("RabbitMQ Setting Error.");
                }
            });

            //Add Redis
            services.AddStackExchangeRedisCache(options =>
            {
                var redisData = services.BuildServiceProvider().GetRequiredService<IOptions<RedisData>>().Value;

                if (redisData != null)
                {
                    string cacheName = redisData.CacheName ?? "";
                    string instanceName = redisData.InstanceName ?? "";

                    options.Configuration = cacheName;
                    options.InstanceName = instanceName;
                }
                else
                {
                    throw new BadHttpRequestException("Redis Setting Error.");
                }
            });

            //Add Hangfire
            services.AddHangfire(config =>
            {
                var hangfireData = services.BuildServiceProvider().GetRequiredService<IOptions<HangfireData>>().Value;

                if (hangfireData != null)
                {
                    string username = hangfireData.Username ?? "";
                    string password = hangfireData.Password ?? "";
                    string dashboardPath = hangfireData.DashboardPath ?? "";
                    string redisConnectionString = hangfireData.RedisConnectionString ?? "";

                    RedisStorageOptions options = new RedisStorageOptions
                    {
                        Prefix = dashboardPath
                    };

                    config.UseRedisStorage(redisConnectionString, options);
                }
                else
                {
                    throw new BadHttpRequestException("Hangfire Setting Error.");
                }
            });

            //Add Service
            services.AddScoped<IErrorExceptionHandler, ErrorExceptionHandler>();
            services.AddScoped<IDataProtected, DataProtected>();
            services.AddScoped<IPasswordHash, PasswordHash>();
            services.AddScoped<IToken, Token>();
            services.AddScoped<IMessagePublish, MessagePublish>();
            services.AddScoped<IMessageConsume, MessageConsume>();
            services.AddScoped<ICache, Cache>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IMessageService, MessageService>();
        }
    }
}

