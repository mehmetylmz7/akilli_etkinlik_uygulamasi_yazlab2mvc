using Microsoft.AspNetCore.Mvc;
using yazlab2mvc.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class AdminController : Controller
{
    private readonly Context _context;

    public AdminController(Context context)
    {
        _context = context;
    }

    // Admin giriş sayfasını render et
    public IActionResult Giris()
    {
        return View();  // /Views/Admin/Giris.cshtml'yi render eder
    }

    // Giriş bilgileri gönderildiğinde çalışacak post action
    [HttpPost]
    public IActionResult Giris(string KullaniciAdi, string Sifre)
    {
        // Admin bilgilerini kontrol et
        if (KullaniciAdi == AdminBilgileri.KullaniciAdi && Sifre == AdminBilgileri.Sifre)
        {
            // Giriş başarılı, admin index sayfasına yönlendir
            return RedirectToAction("Index", "Admin");
        }
        else
        {
            // Giriş başarısız, hata mesajı göster
            ViewBag.ErrorMessage = "Kullanıcı adı veya şifre hatalı!";
            return View(); // Giriş sayfasına hata mesajı ile birlikte geri dön
        }
    }

    // Admin panelinin ana sayfası (admin index)
    public IActionResult Index()
    {
        return View(); // /Views/Admin/Index.cshtml'yi render eder
    }

    // Kullanıcıları listele
    public IActionResult KullaniciListele()
    {
        var kullanicilar = _context.Kullanicilar.ToList(); // Veritabanından kullanıcıları al
        return View(kullanicilar); // Kullanıcıları listelemek için view'a gönder
    }

    // Kullanıcı detaylarını görüntüle
    public IActionResult KullaniciDetay(int id)
    {


        var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.ID == id); // Kullanıcıyı ID'ye göre al
        if (kullanici == null)
        {
            return NotFound(); // Kullanıcı bulunamadıysa hata döndür
        }
        return View(kullanici); // Kullanıcıyı view'a gönder
    }

    // Kullanıcıyı sil
    [HttpPost]
    public IActionResult KullaniciSil(int id)
    {
        var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.ID == id);
        if (kullanici != null)
        {
            _context.Kullanicilar.Remove(kullanici); // Kullanıcıyı veritabanından sil
            _context.SaveChanges(); // Değişiklikleri kaydet
        }
        return RedirectToAction("KullaniciListele"); // Kullanıcılar listesine yönlendir
    }

    // Etkinlikleri listele
    public IActionResult EtkinlikListele()
    {
        var etkinlikler = _context.Etkinlikler.ToList(); // Veritabanından etkinlikleri al
        return View(etkinlikler); // Etkinlikleri view'a gönder
    }


    // Etkinliği onayla
    [HttpPost]
    public IActionResult EtkinlikOnayla(int id)
    {
        // Etkinliği Etkinlikler tablosunda bulalım
        var etkinlik = _context.Etkinlikler.FirstOrDefault(e => e.ID == id);

        if (etkinlik != null)
        {
            etkinlik.EtkinlikDurumu = "Onaylı"; // Etkinlik durumunu onaylı yap
            _context.SaveChanges(); // Değişiklikleri kaydet
        }

        return RedirectToAction("EtkinlikListele"); // Etkinlikler listesine yönlendir
    }

    // Etkinliği reddet
    [HttpPost]
    public IActionResult EtkinlikRed(int id)
    {
        // Etkinliği Etkinlikler tablosunda bulalım
        var etkinlik = _context.Etkinlikler.FirstOrDefault(e => e.ID == id);

        if (etkinlik != null)
        {
            etkinlik.EtkinlikDurumu = "Reddedildi"; // Etkinlik durumunu reddedildi yap
            _context.SaveChanges(); // Değişiklikleri kaydet
        }

        return RedirectToAction("EtkinlikListele"); // Etkinlikler listesine yönlendir
    }

    // Etkinliği sil
    [HttpPost]
    public IActionResult EtkinlikSil(int id)
    {
        var etkinlik = _context.Etkinlikler.FirstOrDefault(e => e.ID == id);

        if (etkinlik != null)
        {
            // Etkinlik ile ilişkili katılımcıları bulup sil
            var katilimcilar = _context.Katilimcilar.Where(k => k.EtkinlikID == id).ToList();
            if (katilimcilar.Any())
            {
                _context.Katilimcilar.RemoveRange(katilimcilar); // Katılımcıları sil
            }

            // Etkinliği sil
            _context.Etkinlikler.Remove(etkinlik);
            _context.SaveChanges(); // Değişiklikleri kaydet
        }

        return RedirectToAction("EtkinlikListele"); // Etkinlikler listesine yönlendir
    }

    // Katılımcıları listele
    public IActionResult KatilimciListele()
    {
        var katilimcilar = _context.Katilimcilar
                                    .Include(k => k.Kullanici) // Kullanıcı bilgilerini dahil et
                                    .Include(k => k.Etkinlik) // Etkinlik bilgilerini dahil et
                                    .ToList(); // Verileri al

        return View(katilimcilar); // Katılımcıları view'a gönder
    }



    // Katılımcıyı sil
    [HttpPost]
    public IActionResult KatilimciSil(int kullaniciID, int etkinlikID)
    {
        var katilimci = _context.Katilimcilar
                                .FirstOrDefault(k => k.KullaniciID == kullaniciID && k.EtkinlikID == etkinlikID);

        if (katilimci != null)
        {
            _context.Katilimcilar.Remove(katilimci); // Katılımcıyı sil
            _context.SaveChanges(); // Değişiklikleri kaydet
        }

        return RedirectToAction("KatilimciListele"); // Katılımcılar listesine yönlendir
    }
}