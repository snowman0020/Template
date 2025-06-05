using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Template.Infrastructure.MySQL.Models;

namespace Template.Infrastructure.MySQL
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
            //Table Users
            //modelBuilder.Entity<Users>().Property(m => m.OrderNumber).ValueGeneratedOnAdd();
            //modelBuilder.Entity<Users>().Property(m => m.OrderNumber).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            modelBuilder.Entity<Users>().Property(m => m.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<Users>().Property(m => m.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<Users>().Property(m => m.CreatedBy).HasDefaultValue("System");

            modelBuilder.Entity<Users>().HasIndex(m => m.Email).IsUnique();
            //modelBuilder.Entity<Users>().HasIndex(m => m.OrderNumber).IsUnique();

            //Table Tokens
            //modelBuilder.Entity<Tokens>().Property(m => m.OrderNumber).ValueGeneratedOnAdd();
            //modelBuilder.Entity<Tokens>().Property(m => m.OrderNumber).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            modelBuilder.Entity<Tokens>().Property(m => m.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<Tokens>().Property(m => m.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<Tokens>().Property(m => m.CreatedBy).HasDefaultValue("System");

            //modelBuilder.Entity<Tokens>().HasIndex(m => m.OrderNumber).IsUnique();

            //Table Messages
            //modelBuilder.Entity<Messages>().Property(m => m.OrderNumber).ValueGeneratedOnAdd();
            //modelBuilder.Entity<Messages>().Property(m => m.OrderNumber).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            modelBuilder.Entity<Messages>().Property(m => m.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<Messages>().Property(m => m.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<Messages>().Property(m => m.CreatedBy).HasDefaultValue("System");
            modelBuilder.Entity<Messages>().Property(m => m.IsSent).HasDefaultValue(false);

            //modelBuilder.Entity<Messages>().HasIndex(m => m.OrderNumber).IsUnique();

            //Table MessageLines
            //modelBuilder.Entity<MessageLines>().Property(m => m.OrderNumber).ValueGeneratedOnAdd();
            //modelBuilder.Entity<MessageLines>().Property(m => m.OrderNumber).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            modelBuilder.Entity<MessageLines>().Property(m => m.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<MessageLines>().Property(m => m.CreatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<MessageLines>().Property(m => m.CreatedBy).HasDefaultValue("System");
            modelBuilder.Entity<MessageLines>().Property(m => m.IsSentSuccess).HasDefaultValue(false);

            //modelBuilder.Entity<MessageLines>().HasIndex(m => m.OrderNumber).IsUnique();

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

        //TODO: OrderNumber=> auto increment it should implement in code
        public TemplateDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json").Build();

            var connectionString = configuration["ConnectionServer"] ?? "";

            var builder = new DbContextOptionsBuilder<TemplateDbContext>();

            var serverVersion = ServerVersion.AutoDetect(connectionString);
            builder.UseMySql(connectionString, serverVersion);

            return new TemplateDbContext(builder.Options);
        }
    }
}