using System.ComponentModel.DataAnnotations;

namespace yazlab2mvc.Models
{
    public class Mesajlar
    {
        [Key]
        public int MesajID { get; set; }
        public int GondericiID { get; set; }
        public int AliciID { get; set; }
        public int EtkinlikID { get; set; }  // Yeni eklenen özellik
        public string MesajMetni { get; set; }
        public DateTime GonderimZamani { get; set; }

        public virtual Kullanicilar Gonderici { get; set; }
        public virtual Kullanicilar Alici { get; set; }
        public virtual Etkinlikler Etkinlik { get; set; }  // Etkinlikler ile ilişki
    }
}

