using Microsoft.EntityFrameworkCore;
using Template.Infrastructure.Models;

namespace Template.Infrastructure
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>().Property(b => b.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<Users>().Property(b => b.CreatedDate).HasDefaultValue(DateTime.Now);
            modelBuilder.Entity<Users>().Property(b => b.CreatedBy).HasDefaultValue("System");

            base.OnModelCreating(modelBuilder);
        }
    }
}
