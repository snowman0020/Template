using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.EntityFrameworkCore.Extensions;
using Template.Infrastructure.MongoDB.Models;

namespace Template.Infrastructure.MongoDB
{
    public class TemplateDbContext : DbContext, IDesignTimeDbContextFactory<TemplateDbContext>
    {
        public TemplateDbContext() { }
        public TemplateDbContext(DbContextOptions<TemplateDbContext> options) : base(options)
        {
        }
        // Registered DB Model in TemplateDbContext file
        public DbSet<Users> Users { get; set; }
        public DbSet<Tokens> Tokens { get; set; }
        public DbSet<Messages> Messages { get; set; }
        public DbSet<MessageLines> MessageLines { get; set; }

        /*
          OnModelCreating mainly used to configured our EF model
          And insert master data if required
         */
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>().ToCollection("Users");
            modelBuilder.Entity<Tokens>().ToCollection("Tokens");
            modelBuilder.Entity<Messages>().ToCollection("Messages");
            modelBuilder.Entity<MessageLines>().ToCollection("MessageLines");


            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            base.OnModelCreating(modelBuilder);
        }

        public TemplateDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json").Build();

            var connectionString = configuration["ConnectionServer"] ?? "";
            var databaseName = configuration["DatabaseName"] ?? "";

            var builder = new DbContextOptionsBuilder<TemplateDbContext>();
            builder.UseMongoDB(connectionString, databaseName);

            return new TemplateDbContext(builder.Options);
        }
    }
}