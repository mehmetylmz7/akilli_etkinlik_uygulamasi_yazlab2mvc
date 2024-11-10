using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace yazlab2mvc.Models

{
    public class Kullanicilar
    {
        [Key]
        public int ID { get; set; }
        public string KullaniciAdi { get; set; }
        public string Sifre { get; set; }
        public string Eposta { get; set; }
        public string Konum { get; set; }
        public string IlgiAlanlari { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public DateTime DogumTarihi { get; set; }
        public string Cinsiyet { get; set; }
        public string TelefonNumarasi { get; set; }
        public string ProfilFotografi { get; set; }

        public virtual ICollection<Katilimcilar> KatildigiEtkinlikler { get; set; }
        public virtual ICollection<Mesajlar> GonderilenMesajlar { get; set; }
        public virtual ICollection<Mesajlar> AlinanMesajlar { get; set; }
        public virtual ICollection<Puanlar> Puanlar { get; set; }
    }
}
