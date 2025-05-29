using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Template.Helper;
using Template.Infrastructure;
using Template.Service.IServices;
using Template.Service.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("appsettings.json", false, true);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddDbContext<TemplateDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["DbConnectionString"], x =>
    {
        x.MigrationsAssembly("Template.Infrastructure");
    });
});

builder.Services.AddScoped<IErrorExceptionHandler, ErrorExceptionHandler>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllersWithViews();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
