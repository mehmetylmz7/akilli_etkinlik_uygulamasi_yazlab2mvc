using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace yazlab2mvc.Models
{
    public class Etkinlikler
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Etkinlik adı gereklidir.")]
        public string EtkinlikAdi { get; set; }

        public string Aciklama { get; set; }

        [Required(ErrorMessage = "Etkinlik tarihi gereklidir.")]
        [DataType(DataType.Date)]
        public DateTime Tarih { get; set; }

        [Required(ErrorMessage = "Etkinlik saati gereklidir.")]
        [DataType(DataType.Time)]
        public TimeSpan Saat { get; set; }

        public TimeSpan EtkinlikSuresi { get; set; }
        public string Konum { get; set; }
        public string Kategori { get; set; }

        public string EtkinlikDurumu {  get; set; } 

        // Etkinliği oluşturan kullanıcı
        public int OlusturanKullaniciID { get; set; }
        public virtual Kullanicilar OlusturanKullanici { get; set; }


        public virtual ICollection<Katilimcilar> Katilimcilar { get; set; }
        public virtual ICollection<Mesajlar> Mesajlar { get; set; }
    }
}
