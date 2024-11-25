using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace yazlab2mvc.Models
{
    public class Kullanicilar
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı gereklidir.")]
        public string KullaniciAdi { get; set; }

        [Required(ErrorMessage = "Şifre gereklidir.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string Sifre { get; set; }

        [Required(ErrorMessage = "E-posta adresi gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Eposta { get; set; }

        public string Konum { get; set; }
        public string IlgiAlanlari { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }

        [Required(ErrorMessage = "Doğum tarihi gereklidir.")]
        [DataType(DataType.Date)]
        public DateTime DogumTarihi { get; set; }

        public string Cinsiyet { get; set; }
        public string TelefonNumarasi { get; set; }
        public string ProfilFotografi { get; set; }

        // Kullanıcının katıldığı etkinlikler
        public virtual ICollection<Katilimcilar> KatildigiEtkinlikler { get; set; }

        // Kullanıcının oluşturduğu etkinlikler
        public virtual ICollection<Etkinlikler> OlusturduguEtkinlikler { get; set; }

        public virtual ICollection<Mesajlar> GonderilenMesajlar { get; set; }
        public virtual ICollection<Mesajlar> AlinanMesajlar { get; set; }
        public virtual ICollection<Puanlar> Puanlar { get; set; }
    }
}
