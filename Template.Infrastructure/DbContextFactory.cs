using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Infrastructure
{
    public class DbContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        private static string connectionString;

        public DatabaseContext CreateDbContext()
        {
            return CreateDbContext(null);
        }

        public DatabaseContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                    .AddJsonFile($"appsettings.json")
                    .Build();

            if (args != null && args.Length > 0)
            {
                connectionString = args[0];
            }
            else
            {
                connectionString = config["DbConnectionString"];
            }

            var builder = new DbContextOptionsBuilder<DatabaseContext>();
            builder.UseSqlServer(connectionString);

            return new DatabaseContext(builder.Options);
        }
    }
}
