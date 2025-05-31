using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text;
using Template.Domain.AppSetting;
using Template.Helper;
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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtData = builder.Configuration.GetSection("JWT").Get<JWTData>();

        if (jwtData != null)
        {
            //Add JWT
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

//Add Service
builder.Services.AddScoped<IErrorExceptionHandler, ErrorExceptionHandler>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddControllersWithViews();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//Add Swagger
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