using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Template.Infrastructure.SQLite.Models;

namespace Template.Infrastructure.SQLite
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
            modelBuilder.Entity<Users>().Property(m => m.OrderNumber).ValueGeneratedOnAdd();
            modelBuilder.Entity<Users>().Property(m => m.OrderNumber).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            modelBuilder.Entity<Users>().Property(m => m.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<Users>().Property(m => m.CreatedDate).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Users>().Property(m => m.CreatedBy).HasDefaultValue("System");
            modelBuilder.Entity<Users>().Property(m => m.UpdatedDate).HasColumnType("datetime");
            modelBuilder.Entity<Users>().Property(m => m.DeletedDate).HasColumnType("datetime");

            modelBuilder.Entity<Users>().HasIndex(m => m.Email).IsUnique();

            //Table Tokens
            modelBuilder.Entity<Tokens>().Property(m => m.OrderNumber).ValueGeneratedOnAdd();
            modelBuilder.Entity<Tokens>().Property(m => m.OrderNumber).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            modelBuilder.Entity<Tokens>().Property(m => m.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<Tokens>().Property(m => m.CreatedDate).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Tokens>().Property(m => m.CreatedBy).HasDefaultValue("System");
            modelBuilder.Entity<Tokens>().Property(m => m.UpdatedDate).HasColumnType("datetime");
            modelBuilder.Entity<Tokens>().Property(m => m.DeletedDate).HasColumnType("datetime");

            //Table Messages
            modelBuilder.Entity<Messages>().Property(m => m.OrderNumber).ValueGeneratedOnAdd();
            modelBuilder.Entity<Messages>().Property(m => m.OrderNumber).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            modelBuilder.Entity<Messages>().Property(m => m.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<Messages>().Property(m => m.CreatedDate).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<Messages>().Property(m => m.CreatedBy).HasDefaultValue("System");
            modelBuilder.Entity<Messages>().Property(m => m.UpdatedDate).HasColumnType("datetime");
            modelBuilder.Entity<Messages>().Property(m => m.DeletedDate).HasColumnType("datetime");
            modelBuilder.Entity<Messages>().Property(m => m.IsSent).HasDefaultValue(false);

            //Table MessageLines
            modelBuilder.Entity<MessageLines>().Property(m => m.OrderNumber).ValueGeneratedOnAdd();
            modelBuilder.Entity<MessageLines>().Property(m => m.OrderNumber).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            modelBuilder.Entity<MessageLines>().Property(m => m.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<MessageLines>().Property(m => m.CreatedDate).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<MessageLines>().Property(m => m.CreatedBy).HasDefaultValue("System");
            modelBuilder.Entity<MessageLines>().Property(m => m.UpdatedDate).HasColumnType("datetime");
            modelBuilder.Entity<MessageLines>().Property(m => m.DeletedDate).HasColumnType("datetime");
            modelBuilder.Entity<MessageLines>().Property(m => m.IsSentSuccess).HasDefaultValue(false);

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

            var connectionString = configuration["ConnectionServer"];

            var builder = new DbContextOptionsBuilder<TemplateDbContext>();
            builder.UseSqlite(connectionString);

            return new TemplateDbContext(builder.Options);
        }
    }
}