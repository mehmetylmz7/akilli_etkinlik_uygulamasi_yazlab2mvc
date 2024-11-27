using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace yazlab2mvc.Models
{
    public class Katilimcilar
    {
        [Key, Column(Order = 0)]
        public int KullaniciID { get; set; }

        [Key, Column(Order = 1)]
        public int EtkinlikID { get; set; }

      

        public virtual Kullanicilar Kullanici { get; set; }
        public virtual Etkinlikler Etkinlik { get; set; }
    }
}
