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
using Template.Helper.DataProtected;
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

builder.Configuration.AddJsonFile("appsettings.json", false, true);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

//Add support to logging with SERILOG
builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    //loggerConfiguration.WriteTo.Console();
    loggerConfiguration.ReadFrom.Configuration(context.Configuration);
});

//Mapping appsetting.json to class
builder.Services.Configure<LoggingData>(builder.Configuration.GetSection("Logging"));
builder.Services.Configure<SerilogData>(builder.Configuration.GetSection("Serilog"));
builder.Services.Configure<JWTData>(builder.Configuration.GetSection("JWT"));
builder.Services.Configure<CustomSettingData>(builder.Configuration.GetSection("CustomSetting"));
builder.Services.Configure<RabbitMQData>(builder.Configuration.GetSection("RabbitMQ"));

//Add JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtData = builder.Configuration.GetSection("JWT").Get<JWTData>();

        if (jwtData != null)
        {
            string issuer = jwtData.Issuer ?? "";
            string key = jwtData.Key ?? "";

            options.RequireHttpsMetadata = false;

            options.SaveToken = true;

            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = false,
                ValidIssuer = issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };
        }
        else
        {
            throw new BadHttpRequestException("Jwt Setting Error.");
        }
    });

//Add DataProtection 
var contentRootPath = builder.Environment.ContentRootPath;

var dataProtectionPath = Path.Combine(contentRootPath, "dataProtectedInformation");

builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(dataProtectionPath)).SetApplicationName("Template").SetDefaultKeyLifetime(TimeSpan.FromDays(90));

//Add Service
builder.Services.AddScoped<IErrorExceptionHandler, ErrorExceptionHandler>();
builder.Services.AddScoped<IDataProtected, DataProtected>();
builder.Services.AddScoped<IPasswordHash, PasswordHash>();
builder.Services.AddScoped<IToken, Token>();
builder.Services.AddScoped<IMessagePublish, MessagePublish>();
builder.Services.AddScoped<IMessageConsume, MessageConsume>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

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

//Add DbContext
builder.Services.AddDbContext<TemplateDbContext>(options =>
{
    var customSettingData = builder.Configuration.GetSection("CustomSetting").Get<CustomSettingData>();

    if (customSettingData != null)
    {
        var dbConnectionStringData =customSettingData.DbConnectionString ?? "";

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

//Add RabbitMQ
builder.Services.AddMassTransit(options =>
{
    var rabbitMQData = builder.Configuration.GetSection("RabbitMQ").Get<RabbitMQData>();

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

// Add CORS
builder.Services.AddCors(options => {
    options.AddPolicy("cors", builder => {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //IF Devlopment Environment can see display swagger
    app.UseSwagger();
    app.UseSwaggerUI();
}

////Add support to logging request with SERILOG
//app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();