using Microsoft.EntityFrameworkCore;
using TrustInsuranceApi1.Models;

namespace TrustInsuranceApi1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Gender> Genders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Seeding لجدول Genders
            modelBuilder.Entity<Gender>().HasData(
                new Gender { Id = 1, Name = "ذكر" },
                new Gender { Id = 2, Name = "أنثى" }
            );
        }
    }
}
