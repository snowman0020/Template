using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Template.Infrastructure.MongoDB;

string fileName = $"appsettings.json";

var configuration = new ConfigurationBuilder().AddJsonFile(fileName).Build();

var connectionString = configuration["ConnectionServer"] ?? "";
var databaseName = configuration["DatabaseName"] ?? "";

var builder = new DbContextOptionsBuilder<TemplateDbContext>();
builder.UseMongoDB(connectionString, databaseName);

var db = new TemplateDbContext(builder.Options);

Console.WriteLine("Database migrating...");

try
{
    //db.Database.Migrate();
    Console.WriteLine("Finished migration...");
}
catch (Exception ex)
{
    Console.WriteLine("Error migration message: " + ex.Message.ToString());
}