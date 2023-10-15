using CustomerServiceCampaignAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerServiceCampaignAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base (options)
        {
        }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Purchase> Purchases { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agent>().HasData(
                new Agent()
                {
                    Id = 1,
                    Name = "John",
                    Surname = "Smith",
                    Email = "john@gmail.com"
                },
                new Agent()
                {
                    Id = 2,
                    Name = "Mark",
                    Surname = "Kenfild",
                    Email = "mark@gmail.com"
                },
                new Agent()
                {
                    Id = 3,
                    Name = "Jack",
                    Surname = "Jonson",
                    Email = "jack@gmail.com"
                }
            );
        }
    }
}
