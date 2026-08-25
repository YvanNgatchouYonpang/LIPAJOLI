using LIPAJOLI.Models;
using Microsoft.EntityFrameworkCore;

namespace LIPAJOLI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Livre> Livres { get; set; }

        public DbSet<Usager> Usagers { get; set; }

        public DbSet<Emprunt> Emprunts { get; set; }

        public DbSet<Exemplaire> Exemplaire { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Livre>().ToTable("Livres");
            modelBuilder.Entity<Usager>().ToTable("Usagers");
            modelBuilder.Entity<Emprunt>().ToTable("Emprunts");
            modelBuilder.Entity<Exemplaire>().ToTable("Exemplaires");
        }

    }
}
