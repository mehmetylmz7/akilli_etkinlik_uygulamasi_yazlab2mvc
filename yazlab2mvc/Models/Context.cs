using Microsoft.EntityFrameworkCore;

namespace yazlab2mvc.Models
{
    public class Context:DbContext 
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=DIDIM\\SQLEXPRESS; database=dbyazlab2; integrated security=true;TrustServerCertificate = True;");
        }

        public DbSet<Kullanicilar> Kullanicilar { get; set; }
        public DbSet<Etkinlikler> Etkinlikler { get; set; }
        public DbSet<Katilimcilar> Katilimcilar { get; set; }
        public DbSet<Mesajlar> Mesajlar { get; set; }
        public DbSet<Puanlar> Puanlar { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Birleşik anahtarları belirtin
            modelBuilder.Entity<Katilimcilar>()
                .HasKey(k => new { k.KullaniciID, k.EtkinlikID });

            modelBuilder.Entity<Puanlar>()
                .HasKey(p => new { p.KullaniciID, p.KazanilanTarih });

            // Kullanıcı-Gönderilen ve Alınan Mesajlar ilişkisi
            modelBuilder.Entity<Mesajlar>()
                .HasOne(m => m.Gonderici)
                .WithMany(k => k.GonderilenMesajlar)
                .HasForeignKey(m => m.GondericiID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Mesajlar>()
                .HasOne(m => m.Alici)
                .WithMany(k => k.AlinanMesajlar)
                .HasForeignKey(m => m.AliciID)
                .OnDelete(DeleteBehavior.Restrict);

            // Etkinlik-Mesajlar ilişkisi
            modelBuilder.Entity<Mesajlar>()
                .HasOne(m => m.Etkinlik)
                .WithMany(e => e.Mesajlar)
                .HasForeignKey(m => m.EtkinlikID);
        }

    }
}
