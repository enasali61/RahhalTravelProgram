using System.Reflection;
using Domain.Entities;
using Domain.Entities.SubEntity;
using Domain.Entities.TripAndPlaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Presistence.Data
{
    public class ApplicationDbContext : IdentityDbContext<Users, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }
        public DbSet<Places> Places { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<UserPlaces> UserPlaces { get; set; }
        public DbSet<PlaceImages> PlaceImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<TripPlace> TripPlaces { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<UserNotificationSettings> UserNotificationSettings { get; set; }
        public DbSet<Scan> Scans { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Table names
            modelBuilder.Entity<Places>().ToTable("Places");
            modelBuilder.Entity<Users>().ToTable("Users");
            modelBuilder.Entity<SubscriptionPlan>(entity =>
            {
                entity.Property(e => e.Price).HasPrecision(18, 2);
            });
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


        }
    }
}

