using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; 
namespace LiftAndAesthetics.Models
{
    public class LiftContext : IdentityDbContext<IdentityUser>
    {
        public LiftContext(DbContextOptions<LiftContext> options) : base(options)
        {
        }

        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Members> Members { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<FitnessClass> FitnessClasses { get; set; }
        public DbSet<ClassReservation> ClassReservations { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<NutritionPlan> NutritionPlans { get; set; }
        public DbSet<UserMembership> UserMemberships { get; set; }
        public DbSet<ChatMessage> ChatMessage { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}