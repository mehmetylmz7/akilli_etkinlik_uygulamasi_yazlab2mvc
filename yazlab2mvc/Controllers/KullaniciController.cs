using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using yazlab2mvc.Models;

namespace yazlab2mvc.Controllers
{
    public class KullaniciController : Controller
    {
        private readonly Context _context;

        public KullaniciController(Context context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult KayitOl()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> KayitOl(Kullanicilar kullanici)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    // Kullanıcı bilgilerini veritabanına kaydet
                    _context.Kullanicilar.Add(kullanici);
                    await _context.SaveChangesAsync();

                    ViewBag.Message = "Kullanıcı başarıyla eklendi!";
                    ViewBag.IsSuccess = true; // Başarı durumu
                }
                catch (Exception ex)
                {
                    // Hata durumunda ViewBag.Message'e hata mesajını ekleyin
                    ViewBag.Message = "Kayıt sırasında bir hata oluştu: " + ex.Message;
                    ViewBag.IsSuccess = false; // Başarı durumu false
                }

                //return View();
            }
            else
            {
                ViewBag.Message = "Model State is invalid!";
                ViewBag.IsSuccess = false; // Başarı durumu false
            }

            return View(kullanici); // Model doğrulama hataları varsa formu tekrar göster
        }
        // GET: Kullanici/GirisYap
        [HttpGet]
        public IActionResult GirisYap()
        {
            return View();
        }

        // POST: Kullanici/GirisYap
        [HttpPost]
        public IActionResult GirisYap(string kullaniciAdi, string sifre)
        {
            // Kullanıcı adı ve şifreyi veritabanında kontrol et
            var kullanici = _context.Kullanicilar
                .FirstOrDefault(k => k.KullaniciAdi == kullaniciAdi && k.Sifre == sifre);

            if (kullanici != null)
            {
                ViewBag.Message = "Giriş başarılı!";
                ViewBag.IsSuccess = true;

                // Burada kullanıcının giriş yaptığını belirten bir işlem yapabilirsiniz,
                // örneğin oturum açmak için Session kullanabilirsiniz.
            }
            else
            {
                ViewBag.Message = "Kullanıcı adı veya şifre yanlış. Lütfen tekrar deneyin veya kayıt olun.";
                ViewBag.IsSuccess = false;
                return RedirectToAction("KayitOl"); // Kullanıcı bulunamazsa KayitOl sayfasına yönlendir
            }

            return View();
        }
    }
}