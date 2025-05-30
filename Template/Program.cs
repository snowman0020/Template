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

//Add Entity Framework
builder.Services.AddDbContext<TemplateDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["DbConnectionString"], x =>
    {
        x.MigrationsAssembly("Template.Infrastructure");
    });
});

//Add JWT
string jwtIssuer = builder.Configuration["JWT:Issuer"] ?? "";
string jwtKey = builder.Configuration["JWT:Key"] ?? "";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(jwt =>
    {
        jwt.RequireHttpsMetadata = false;
        jwt.SaveToken = true;
        jwt.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateAudience = false,
            ValidIssuer = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

//Add Service
builder.Services.AddScoped<IErrorExceptionHandler, ErrorExceptionHandler>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddControllersWithViews();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//Add Swagger
builder.Services.AddSwaggerGen(sw =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    sw.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    sw.SwaggerDoc("v1", new OpenApiInfo { Title = "Template API", Version = "v1" });

    sw.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: Bearer 12345abcdef",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    sw.DescribeAllParametersInCamelCase();
    sw.OperationFilter<OperationFilter>();
});

//Mapping appsetting.json to class
builder.Services.Configure<LoggingData>(builder.Configuration.GetSection("Logging"));
builder.Services.Configure<SerilogData>(builder.Configuration.GetSection("Serilog"));
builder.Services.Configure<JWTData>(builder.Configuration.GetSection("JWT"));
builder.Services.Configure<AllowedHostsData>(builder.Configuration.GetSection("AllowedHosts"));
builder.Services.Configure<DbConnectionStringData>(builder.Configuration.GetSection("DbConnectionString"));

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