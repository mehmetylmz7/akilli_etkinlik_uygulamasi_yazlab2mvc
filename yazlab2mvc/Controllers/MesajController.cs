using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using yazlab2mvc.Models;

namespace yazlab2mvc.Controllers
{
    public class MesajController : Controller
    {
        private readonly Context _context;

        public MesajController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult MesajlariGetir(int etkinlikID)
        {
            var mesajlar = _context.Mesajlar
                .Where(m => m.EtkinlikID == etkinlikID)
                .Include(m => m.Gonderici) // Gönderen kullanıcı bilgisi
                .Select(m => new
                {
                    GondericiAdi = m.Gonderici.Ad + " " + m.Gonderici.Soyad, // Gönderen kullanıcı adı ve soyadı
                    MesajMetni = m.MesajMetni,
                    GonderimZamani = m.GonderimZamani
                })
                .ToList();

            return Json(mesajlar); // Mesajları JSON formatında döndür
        }


        [HttpPost]
        public IActionResult MesajGonder(int etkinlikID, string mesajMetni)
        {
           /* if (string.IsNullOrWhiteSpace(mesajMetni))
            {
                return Json(new { success = false, message = "Mesaj metni boş olamaz. Lütfen bir mesaj yazın." });
            }
           */
            var kullaniciID = HttpContext.Session.GetInt32("KullaniciID");

            if (!kullaniciID.HasValue)
            {
                return Json(new { success = false, message = "Kullanıcı giriş yapmamış. Lütfen giriş yapın." });
            }

            var mesaj = new Mesajlar
            {
                EtkinlikID = etkinlikID,
                GondericiID = kullaniciID.Value,
                MesajMetni = mesajMetni,
                GonderimZamani = DateTime.Now
            };

            try
            {
                _context.Mesajlar.Add(mesaj);
                _context.SaveChanges();
                return Json(new { success = true, message = "Mesaj başarıyla gönderildi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Bir hata oluştu: {ex.Message}" });
            }
        }



    }
}
