using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Template.Infrastructure.Models;

namespace Template.Infrastructure
{
    public class TemplateDbContext : DbContext, IDesignTimeDbContextFactory<TemplateDbContext>
    {
        public TemplateDbContext() { }
        public TemplateDbContext(DbContextOptions<TemplateDbContext> options) : base(options)
        {
        }
        // Registered DB Model in TemplateDbContext file
        public DbSet<Users> Users { get; set; }

        /*
          OnModelCreating mainly used to configured our EF model
          And insert master data if required
         */
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>().Property(m => m.OrderNumber).ValueGeneratedOnAddOrUpdate();
            modelBuilder.Entity<Users>().Property(m => m.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<Users>().Property(m => m.CreatedDate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Users>().Property(m => m.CreatedBy).HasDefaultValue("System");

            //// Inserting record in User table
            //var user = new Users()
            //{
            //    ID = Guid.NewGuid().ToString(),
            //    FirstName = "admin",
            //    LastName = "admin",
            //    Email = "admin@admin.com",
            //    Phone = "0987654321",
            //};

            //modelBuilder.Entity<Users>().HasData(user);

            base.OnModelCreating(modelBuilder);
        }

        public TemplateDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json").Build();

            var connectionString = configuration["DbConnectionString"];

            var builder = new DbContextOptionsBuilder<TemplateDbContext>();
            builder.UseSqlServer(connectionString);

            return new TemplateDbContext(builder.Options);
        }
    }
}