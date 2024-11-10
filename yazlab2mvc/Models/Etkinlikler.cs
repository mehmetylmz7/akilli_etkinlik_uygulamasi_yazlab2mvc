using System.ComponentModel.DataAnnotations;

namespace yazlab2mvc.Models
{
    public class Etkinlikler
    {
        [Key]
        public int ID { get; set; }
        public string EtkinlikAdi { get; set; }
        public string Aciklama { get; set; }
        public DateTime Tarih { get; set; }
        public TimeSpan Saat { get; set; }
        public TimeSpan EtkinlikSuresi { get; set; }
        public string Konum { get; set; }
        public string Kategori { get; set; }

        public virtual ICollection<Katilimcilar> Katilimcilar { get; set; }
        public virtual ICollection<Mesajlar> Mesajlar { get; set; }
    }
}
