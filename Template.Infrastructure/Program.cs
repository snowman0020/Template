using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Template.Infrastructure;

string fileName = $"appsettings.json";

var configuration = new ConfigurationBuilder()
  .AddJsonFile(fileName)
  .Build();
var connectionString = configuration["DbConnectionString"];

var builder = new DbContextOptionsBuilder<DatabaseContext>();
builder.UseSqlServer(connectionString);
DatabaseContext db = new DatabaseContext(builder.Options);
Console.WriteLine("Applying [Database.Models] database migration.");
db.Database.Migrate();
Console.WriteLine("Finished [Database.Models] database migration.");