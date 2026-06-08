using Microsoft.EntityFrameworkCore;
using Openride.Models;

namespace Openride.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Driver> Drivers => Set<Driver>();
        public DbSet<Ride> Rides => Set<Ride>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User
            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(u => u.Id);
                e.HasIndex(u => u.Phone).IsUnique();
                e.HasIndex(u => u.Email).IsUnique();
            });

            // Driver — one to one with User
            modelBuilder.Entity<Driver>(e =>
            {
                e.HasKey(d => d.Id);
                e.HasOne(d => d.User)
                 .WithOne(u => u.Driver)
                 .HasForeignKey<Driver>(d => d.UserId);
            });

            // Ride
            modelBuilder.Entity<Ride>(e =>
            {
                e.HasKey(r => r.Id);
                e.HasOne(r => r.Rider)
                 .WithMany(u => u.Rides)
                 .HasForeignKey(r => r.RiderId)
                 .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(r => r.Driver)
                 .WithMany(d => d.Rides)
                 .HasForeignKey(r => r.DriverId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}