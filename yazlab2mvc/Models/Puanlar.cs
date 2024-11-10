using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace yazlab2mvc.Models
{
    public class Puanlar
    {
        [Key, Column(Order = 0)]
        public int KullaniciID { get; set; }

        [Key, Column(Order = 1)]
        public DateTime KazanilanTarih { get; set; }

        public int Puan { get; set; }

        public virtual Kullanicilar Kullanici { get; set; }
    }
}
