using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RailConnect.Models;

namespace RailConnect.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Karta> Karta { get; set; }
        public DbSet<Putovanje> Putovanje { get; set; }
        public DbSet<StanicaPutovanje> StanicaPutovanje { get; set; }
        public DbSet<Recenzija> Recenzija { get; set; }
        public DbSet<StanicaPolazak> StanicaPolazak { get; set; }
        public DbSet<StanicaDolazak> StanicaDolazak { get; set; }
        public DbSet<Voz> Voz { get; set; }
        public DbSet<Grad> Grad { get; set; }

        public DbSet<ApplicationUser> ApplicationUser{ get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Karta>().ToTable("Karta");
            modelBuilder.Entity<StanicaPolazak>().ToTable("StanicaPolazak");
            modelBuilder.Entity<StanicaDolazak>().ToTable("StanicaDolazak");
            modelBuilder.Entity<Putovanje>().ToTable("Putovanje");
            modelBuilder.Entity<StanicaPutovanje>().ToTable("StanicaPutovanje");
            modelBuilder.Entity<Recenzija>().ToTable("Recenzija");
            modelBuilder.Entity<Voz>().ToTable("Voz");
            modelBuilder.Entity<Grad>().ToTable("Grad");
            modelBuilder.Entity<ApplicationUser>(b =>
            {
                b.Property(u => u.Ime);
                b.Property(u => u.Prezime);
                b.Property(u => u.Slika);
                b.Property(u => u.BrojKartice);
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
