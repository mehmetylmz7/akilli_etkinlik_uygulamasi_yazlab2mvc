using yazlab2mvc.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace yazlab2mvc
{
    public class KullaniciAdiGetirme
    {
        private readonly Context _context;

        // Dependency Injection kullanılarak veritabanı bağlamı dışarıdan enjekte ediliyor
        public KullaniciAdiGetirme(Context context)
        {
            _context = context;
        }

        public string GetKullaniciAdiById(int kullaniciID)
        {
            try
            {
                // Kullanıcıyı veritabanından al
                var kullanici = _context.Kullanicilar
                    .FirstOrDefault(k => k.ID == kullaniciID);

                // Kullanıcı bulunamazsa veya KullaniciAdi boşsa uygun mesaj döndür
                if (kullanici == null || string.IsNullOrEmpty(kullanici.KullaniciAdi))
                    return "Bilinmeyen Kullanıcı";

                // Kullanıcı adı döndür
                return kullanici.KullaniciAdi;
            }
            catch (Exception ex)
            {
                // Loglama veya başka bir hata yönetimi eklenebilir
                return $"Hata oluştu: {ex.Message}";
            }
        }
    }
}
