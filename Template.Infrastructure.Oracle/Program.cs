using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Template.Infrastructure;

string fileName = $"appsettings.json";

var configuration = new ConfigurationBuilder().AddJsonFile(fileName).Build();

var connectionString = configuration["ConnectionServer"];

var builder = new DbContextOptionsBuilder<TemplateDbContext>();
builder.UseOracle(connectionString);

var db = new TemplateDbContext(builder.Options);

Console.WriteLine("Database migrating...");

try
{
    db.Database.Migrate();
    Console.WriteLine("Finished migration...");
}
catch (Exception ex)
{
    Console.WriteLine("Error migration message: " + ex.Message.ToString());
}