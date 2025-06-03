using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text;
using Template.Domain.AppSetting;
using Template.Helper.DataCache;
using Template.Helper.DataProtected;
using Template.Helper.Email;
using Template.Helper.ErrorException;
using Template.Helper.MessageConsume;
using Template.Helper.MessagePublish;
using Template.Helper.OperationFilter;
using Template.Helper.PasswordHash;
using Template.Helper.Token;
using Template.Infrastructure;
using Template.Service.IServices;
using Template.Service.Services;

var builder = WebApplication.CreateBuilder(args);

var contentRootPath = builder.Environment.ContentRootPath;

builder.Configuration.AddJsonFile("appsettings.json", false, true);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

//Mapping appsetting.json to class
builder.Services.Configure<CustomSettingData>(builder.Configuration.GetSection("CustomSetting"));
builder.Services.Configure<EmailData>(builder.Configuration.GetSection("Email"));
builder.Services.Configure<HangfireData>(builder.Configuration.GetSection("Hangfire"));
builder.Services.Configure<JWTData>(builder.Configuration.GetSection("JWT"));
builder.Services.Configure<LoggingData>(builder.Configuration.GetSection("Logging"));
builder.Services.Configure<RabbitMQData>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<RedisData>(builder.Configuration.GetSection("Redis"));
builder.Services.Configure<SerilogData>(builder.Configuration.GetSection("Serilog"));

//Add Service
//builder.Services.AddScoped<IBackgroundWorkJob, BackgroundWorkJob>();
builder.Services.AddScoped<IDataCache, DataCache>();
builder.Services.AddScoped<IDataProtected, DataProtected>();
builder.Services.AddScoped<IEmail, Email>();
builder.Services.AddScoped<IErrorExceptionHandler, ErrorExceptionHandler>();
builder.Services.AddScoped<IMessageConsume, MessageConsume>();
builder.Services.AddScoped<IMessagePublish, MessagePublish>();
builder.Services.AddScoped<IPasswordHash, PasswordHash>();
builder.Services.AddScoped<IToken, Token>();

builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

//Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", po =>
    {
        po.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

//Add DataProtection 
var dataProtectionPath = Path.Combine(contentRootPath, "dataProtectedInformation");
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath)).SetApplicationName("Template").SetDefaultKeyLifetime(TimeSpan.FromDays(90));

//Add DbContext
builder.Services.AddDbContext<TemplateDbContext>(options =>
{
    string connectionSQLServer = "";

    var customSettingData = builder.Configuration.GetSection("CustomSetting").Get<CustomSettingData>();

    if (customSettingData != null)
    {
        connectionSQLServer = customSettingData.ConnectionSQLServer ?? "";
    }
    else
    {
        throw new BadHttpRequestException("custom Setting Error.");
    }

    options.UseSqlServer(connectionSQLServer, s =>
    {
        s.MigrationsAssembly("Template.Infrastructure");
    });
});

//Add JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        string jwtIssuer = "";
        string jwtKey = "";

        var jwtData = builder.Configuration.GetSection("JWT").Get<JWTData>();

        if (jwtData != null)
        {
            jwtIssuer = jwtData.Issuer ?? "";
            jwtKey = jwtData.Key ?? "";
        }
        else
        {
            throw new BadHttpRequestException("Jwt Setting Error.");
        }

        options.RequireHttpsMetadata = false;

        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = false,
            ValidIssuer = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

//Add RabbitMQ
builder.Services.AddMassTransit(options =>
{
    string rabbitHostName = "";
    string rabbitUsername = "";
    string rabbitPassword = "";
    string rabbitQueueName = "";

    var rabbitMQData = builder.Configuration.GetSection("RabbitMQ").Get<RabbitMQData>();

    if (rabbitMQData != null)
    {
        rabbitHostName = rabbitMQData.HostName ?? "";
        rabbitUsername = rabbitMQData.UserName ?? "";
        rabbitPassword = rabbitMQData.Password ?? "";
        rabbitQueueName = rabbitMQData.QueueName ?? "";
    }
    else
    {
        throw new BadHttpRequestException("RabbitMQ Setting Error.");
    }

    options.AddConsumer<MessageConsume>();
    options.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitHostName, "/", h =>
        {
            h.Username(rabbitUsername);
            h.Password(rabbitPassword);
        });
        cfg.ReceiveEndpoint(rabbitQueueName, r =>
        {
            r.ConfigureConsumer<MessageConsume>(context);
        });
    });
});

//Add Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    string redisInstanceName = "";
    string redisConfigurationServer = "";

    var redisData = builder.Configuration.GetSection("Redis").Get<RedisData>();
    if (redisData != null)
    {
        redisInstanceName = redisData.InstanceName ?? "";
        redisConfigurationServer = redisData.ConnectionServer ?? "";
    }
    else
    {
        throw new BadHttpRequestException("Redis Setting Error.");
    }

    options.InstanceName = redisInstanceName;
    options.Configuration = redisConfigurationServer;
});

//Add SERILOG
builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    //loggerConfiguration.WriteTo.Console();
    loggerConfiguration.ReadFrom.Configuration(context.Configuration);
});

//Add Swagger
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));

    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Template API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: Bearer 12345abcdef",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    options.DescribeAllParametersInCamelCase();
    options.OperationFilter<OperationFilter>();
});

////Add Hangfire
//string hangfireUsername = "";
//string hangfirePassword = "";
//string hangfireDashboardPath = "";

//string prefixName = "";
//string redisConnectionString = "";

//var hangfireData = builder.Configuration.GetSection("Hangfire").Get<HangfireData>();

//if (hangfireData != null)
//{
//    hangfireUsername = hangfireData.Username ?? "";
//    hangfirePassword = hangfireData.Password ?? "";
//    hangfireDashboardPath = hangfireData.DashboardPath ?? "";

//    prefixName = hangfireData.PrefixName ?? "";
//    redisConnectionString = hangfireData.RedisConnectionString ?? "";
//}
//else
//{
//    throw new BadHttpRequestException("Hangfire Setting Error.");
//}

//var redis = ConnectionMultiplexer.Connect(redisConnectionString);

//builder.Services.AddHangfire(config =>
//{
//    config.UseRedisStorage(redis);
//});

//builder.Services.AddHangfireServer();
////builder.Services.AddHostedService<TestService>;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //IF Devlopment Environment can see display swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

////Add support to logging request with SERILOG
//app.UseSerilogRequestLogging();

app.Run();



////Use Hangfire
//var options = new DashboardOptions
//{
//    Authorization = new[] { new BasicAuthAuthorizationFilter(
//                           new BasicAuthAuthorizationFilterOptions
//                            {
//                               SslRedirect = false,
//                               RequireSsl = false,
//                               LoginCaseSensitive = true,
//                               Users = new []
//                               {
//                                    new BasicAuthAuthorizationUser()
//                                    {
//                                        Login = hangfireUsername,
//                                        PasswordClear = hangfirePassword
//                                    }
//                               }
//                            })
//                         }
//};

//app.UseHangfireDashboard("/hangfire");
