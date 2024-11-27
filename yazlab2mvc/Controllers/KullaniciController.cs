using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
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

        // Profilim Sayfası
        public IActionResult Profil()
        {
            var kullaniciID = HttpContext.Session.GetInt32("KullaniciID");
            var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.ID == kullaniciID);

            if (kullanici != null)
            {
                ViewData["KullaniciAdi"] = kullanici.Ad + " " + kullanici.Soyad; // Kullanıcı adı ve soyadı birleştirip ViewData'ya ekliyoruz.
            }

            return View(kullanici); // Profil sayfasını döndürüyoruz
        }


    
        // Puan Geçmişim
        public IActionResult Puanlar()
        {
            var kullaniciID = HttpContext.Session.GetInt32("KullaniciID");
            var puanlar = _context.Puanlar.Where(p => p.KullaniciID == kullaniciID).ToList();
            return View(puanlar);
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

        public IActionResult KullaniciArayuz()
        {
            var kullaniciID = HttpContext.Session.GetInt32("KullaniciID");

            if (kullaniciID.HasValue)
            {
                var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.ID == kullaniciID.Value);

                if (kullanici != null)
                {
                    // Kullanıcı adı ve soyadını ViewData'ya aktarıyoruz
                    ViewData["KullaniciAdi"] = kullanici.Ad + " " + kullanici.Soyad;
                }
            }

            return View(); // Kullanıcı arayüzüne yönlendir
        }





        // POST: Kullanici/GirisYap
        [HttpPost]
        public IActionResult GirisYap(string kullaniciAdi, string sifre)
        {
            var kullanici = _context.Kullanicilar
                .FirstOrDefault(k => k.KullaniciAdi == kullaniciAdi && k.Sifre == sifre);

            if (kullanici != null)
            {
                // Kullanıcı ID'sini oturuma ekle
                HttpContext.Session.SetInt32("KullaniciID", kullanici.ID);

                // Başarılı girişten sonra yönlendir
                return RedirectToAction("KullaniciArayuz", "Kullanici");
            }
            else
            {
                ViewBag.Message = "Kullanıcı adı veya şifre yanlış. Lütfen tekrar deneyin.";
                ViewBag.IsSuccess = false;
                return View();
            }
        }
        [HttpGet]
        public IActionResult EtkinlikOlustur()
        {
            return View();
        }




        [HttpPost]
        public async Task<IActionResult> EtkinlikOlustur(Etkinlikler etkinlik)
        {
            try
            {
                // Giriş yapan kullanıcının ID'sini oturumdan al
                var kullaniciID = HttpContext.Session.GetInt32("KullaniciID");
                if (!kullaniciID.HasValue)
                {
                    ViewBag.Message = "Kullanıcı giriş yapmamış. Lütfen giriş yapın.";
                    return RedirectToAction("GirisYap", "Kullanici");
                }

                // Kullanıcı ID'sini etkinlik ile eşleştir
                etkinlik.OlusturanKullaniciID = kullaniciID.Value;

                // EtkinlikDurumu'nu otomatik olarak "Onay Bekliyor" şeklinde ata
                etkinlik.EtkinlikDurumu = "Onay Bekliyor";

                // Etkinliği veritabanına ekle
                _context.Etkinlikler.Add(etkinlik);
                await _context.SaveChangesAsync(); // SaveChanges sonrası etkinlik ID'si oluşturulur

                // Katilimcilar tablosuna etkinlik oluşturucusunu ekle
                var katilimci = new Katilimcilar
                {
                    KullaniciID = kullaniciID.Value,
                    EtkinlikID = etkinlik.ID
                };
                _context.Katilimcilar.Add(katilimci);
                await _context.SaveChangesAsync();

                ViewBag.Message = "Etkinlik başarıyla oluşturuldu ve katılım eklendi!";
                return RedirectToAction("KullaniciArayuz", "Kullanici");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Bir hata oluştu: " + ex.Message;
                return View(etkinlik);
            }
        }

        [HttpPost]
        public IActionResult EtkinligeKatil(int etkinlikID)
        {
            var kullaniciID = HttpContext.Session.GetInt32("KullaniciID");

            if (!kullaniciID.HasValue)
            {
                ViewBag.Message = "Kullanıcı giriş yapmamış. Lütfen giriş yapın.";
                return RedirectToAction("GirisYap", "Kullanici");
            }

            // Kullanıcı daha önce bu etkinliğe katılmış mı kontrol ediliyor
            var mevcutKatilim = _context.Katilimcilar
                .FirstOrDefault(k => k.KullaniciID == kullaniciID.Value && k.EtkinlikID == etkinlikID);

            if (mevcutKatilim == null)
            {
                // Katılım bilgisi ekleniyor
                var yeniKatilim = new Katilimcilar
                {
                    KullaniciID = kullaniciID.Value,
                    EtkinlikID = etkinlikID
                };

                _context.Katilimcilar.Add(yeniKatilim);
                _context.SaveChanges();

                ViewBag.Message = "Etkinliğe başarıyla katıldınız, onay bekliyorsunuz.";
            }
            else
            {
                ViewBag.Message = "Bu etkinliğe zaten katıldınız.";
            }

            return RedirectToAction("EtkinlikleriGoruntule");
        }


        public IActionResult OnayBekleyenEtkinlikler()
        {
            var kullaniciID = HttpContext.Session.GetInt32("KullaniciID");

            if (!kullaniciID.HasValue)
            {
                // Kullanıcı giriş yapmamışsa, giriş yapmasını isteyelim
                return RedirectToAction("GirisYap", "Kullanici");
            }

            // Kullanıcının oluşturduğu ve durumu "Onay Bekliyor" olan etkinlikleri alıyoruz
            var etkinlikler = _context.Etkinlikler
                                      .Where(e => e.OlusturanKullaniciID == kullaniciID.Value &&
                                                  e.EtkinlikDurumu == "Onay Bekliyor")
                                      .ToList();

            return View(etkinlikler); // View'da bu etkinlikleri göstereceğiz
        }

        public IActionResult ReddedilenEtkinlikler()
        {
            var kullaniciID = HttpContext.Session.GetInt32("KullaniciID");

            if (!kullaniciID.HasValue)
            {
                // Kullanıcı giriş yapmamışsa, giriş yapmasını isteyelim
                return RedirectToAction("GirisYap", "Kullanici");
            }

            // Kullanıcının oluşturduğu ve durumu "Reddedildi" olan etkinlikleri alıyoruz
            var etkinlikler = _context.Etkinlikler
                                      .Where(e => e.OlusturanKullaniciID == kullaniciID.Value &&
                                                  e.EtkinlikDurumu == "Reddedildi")
                                      .ToList();

            return View(etkinlikler); // View'da bu etkinlikleri göstereceğiz
        }


        public IActionResult EtkinlikleriGoruntule()
        {
            // Kullanıcı ID'sini session'dan al
            var kullaniciID = HttpContext.Session.GetInt32("KullaniciID");

            // Eğer kullanıcı ID'si null ise, giriş yapmamış demektir
            if (kullaniciID == null)
            {
                // Kullanıcı giriş yapmamışsa, uygun bir yönlendirme veya hata mesajı ekleyebilirsiniz
                return RedirectToAction("GirisYap", "Kullanici");
            }

            // Katilimcilar tablosunda bu kullanıcının katılmadığı ve EtkinlikDurumu = "Onaylandı" olan etkinlikleri almak
            var etkinlikler = _context.Etkinlikler
                                    .Where(e => e.EtkinlikDurumu == "Onaylandı" && // Sadece "Onaylandı" olan etkinlikler
                                                !_context.Katilimcilar
                                                    .Any(k => k.KullaniciID == kullaniciID && k.EtkinlikID == e.ID))
                                    .ToList();

            return View(etkinlikler);
        }


        public IActionResult KatildigimEtkinlikler()
        {
            var kullaniciID = HttpContext.Session.GetInt32("KullaniciID");

            if (!kullaniciID.HasValue)
            {
                return RedirectToAction("GirisYap", "Kullanici"); // Giriş yapmamış kullanıcıyı yönlendir
            }

            // Katılım durumu "Katılıyor" ve Etkinlik durumu "Onaylandı" olan etkinlikleri al
            var etkinlikler = _context.Katilimcilar
                                       .Where(k => k.KullaniciID == kullaniciID.Value &&
                                                   k.Etkinlik.EtkinlikDurumu == "Onaylandı")
                                       .Select(k => k.Etkinlik) // Katılım ile ilişkili etkinlikleri al
                                       .ToList();

            return View(etkinlikler); // Etkinlikleri View'e gönder
        }


        [HttpGet]
        public IActionResult MesajGonder(int etkinlikID)
        {
            // Etkinliği al
            var etkinlik = _context.Etkinlikler.FirstOrDefault(e => e.ID == etkinlikID);

            if (etkinlik == null)
            {
                TempData["ErrorMessage"] = "Etkinlik bulunamadı.";
                return RedirectToAction("KatildigimEtkinlikler", "Kullanici");
            }

            // Etkinlikle ilgili mesajları al
            var mesajlar = _context.Mesajlar
                .Where(m => m.EtkinlikID == etkinlikID)
                .OrderByDescending(m => m.GonderimZamani) // Tarihe göre sıralama
                .ToList();

            // ViewData ile etkinlik bilgisi ve mesajları gönder
            ViewData["EtkinlikAdi"] = etkinlik.EtkinlikAdi;
            ViewData["EtkinlikID"] = etkinlikID;
            ViewData["Mesajlar"] = mesajlar;

            return View();
        }



        [HttpPost]
        public IActionResult MesajGonder(int etkinlikID, string mesaj)
        {
            var kullaniciID = HttpContext.Session.GetInt32("KullaniciID");

            if (!kullaniciID.HasValue)
            {
                ViewBag.Message = "Kullanıcı giriş yapmamış. Lütfen giriş yapın.";
                return RedirectToAction("GirisYap", "Kullanici");
            }

            // Mesajı kaydet
            var yeniMesaj = new Mesajlar
            {
                GondericiID = kullaniciID.Value,
                AliciID = kullaniciID.Value, // Alıcı yok
                EtkinlikID = etkinlikID,
                MesajMetni = mesaj,
                GonderimZamani = DateTime.Now
            };

            _context.Mesajlar.Add(yeniMesaj);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Mesaj başarıyla gönderildi!";
            return RedirectToAction("MesajGonder", new { etkinlikID = etkinlikID });
        }


        // Şifre yenileme sayfası GET
        [HttpGet]
        public IActionResult SifreYenileme()
        {
            return View();
        }

        // Şifre yenileme POST işlemi
        [HttpPost]
        public async Task<IActionResult> SifreYenileme(string kullaniciAdi, string eposta, string telefonNumarasi, string yeniSifre)
        {
            // Formdaki alanların boş olup olmadığını kontrol ediyoruz
            if (string.IsNullOrEmpty(kullaniciAdi) || string.IsNullOrEmpty(eposta) || string.IsNullOrEmpty(telefonNumarasi) || string.IsNullOrEmpty(yeniSifre))
            {
                ViewBag.Message = "Tüm alanları doldurduğunuzdan emin olun.";
                ViewBag.IsSuccess = false;
                return View();
            }

            // Kullanıcı adı, E-posta ve Telefon Numarası ile kullanıcıyı veritabanında arıyoruz
            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(u => u.KullaniciAdi == kullaniciAdi && u.Eposta == eposta && u.TelefonNumarasi == telefonNumarasi);

            // Kullanıcı bulunamazsa hata mesajı gösteriyoruz
            if (kullanici == null)
            {
                ViewBag.Message = "Bu bilgilerle kayıtlı kullanıcı bulunamadı.";
                ViewBag.IsSuccess = false;
                return View();
            }

            // Yeni şifreyi güncelliyoruz
            kullanici.Sifre = yeniSifre;

            try
            {
                _context.Update(kullanici);
                await _context.SaveChangesAsync();

                ViewBag.Message = "Şifreniz başarıyla güncellenmiştir.";
                ViewBag.IsSuccess = true;

                // Kullanıcıyı giriş sayfasına yönlendiriyoruz
                return RedirectToAction("GirisYap", "Kullanici");
            }
            catch (Exception ex)
            {
                // Hata durumu
                ViewBag.Message = "Şifre güncellenirken bir hata oluştu: " + ex.Message;
                ViewBag.IsSuccess = false;
                return View();
            }
        }


        [HttpGet]
        public IActionResult ProfilGuncelle()
        {
            var kullaniciID = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciID == null)
            {
                return RedirectToAction("GirisYap", "Kullanici");
            }

            var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.ID == kullaniciID);
            if (kullanici == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            return View(kullanici);
        }

        [HttpPost]
        public async Task<IActionResult> ProfilGuncelle(Kullanicilar guncelKullanici)
        {
            var kullaniciID = HttpContext.Session.GetInt32("KullaniciID");
            if (kullaniciID == null)
            {
                return RedirectToAction("GirisYap", "Kullanici");
            }

            var mevcutKullanici = _context.Kullanicilar.FirstOrDefault(k => k.ID == kullaniciID);
            if (mevcutKullanici == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            // Güncellemeleri uygula
            mevcutKullanici.Ad = guncelKullanici.Ad;
            mevcutKullanici.Soyad = guncelKullanici.Soyad;
            mevcutKullanici.Eposta = guncelKullanici.Eposta;
            mevcutKullanici.TelefonNumarasi = guncelKullanici.TelefonNumarasi;
            mevcutKullanici.Konum = guncelKullanici.Konum;

            try
            {
                await _context.SaveChangesAsync();
                ViewBag.Message = "Bilgileriniz başarıyla güncellendi.";
                ViewBag.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Güncelleme sırasında bir hata oluştu: " + ex.Message;
                ViewBag.IsSuccess = false;
            }

            return View(mevcutKullanici);
        }

    }
}