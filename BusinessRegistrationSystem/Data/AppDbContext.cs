using BusinessRegistrationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessRegistrationSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<BusinessRegistration> BusinessRegistrations { get; set; }
        public DbSet<Director> Directors { get; set; }
        public DbSet<Shareholder> Shareholders { get; set; }
        public DbSet<Secretary> Secretaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.Role).IsRequired();
            });

            // Seed hardcoded admin user: admin / admin123
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Username = "sysadmin",
                PasswordHash = "4PrvXIsA6S8uQAsR4YgIpkpBy2y3w6u0FIn959d8pY4=", // Base64 of SHA256("admin123")
                Role = UserRole.Admin
            });
        }
    }
}
