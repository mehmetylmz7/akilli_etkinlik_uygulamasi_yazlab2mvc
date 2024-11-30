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
            // Kullanıcı oturum kontrolü
            var kullaniciID = HttpContext.Session.GetInt32("KullaniciID");
            if (!kullaniciID.HasValue)
            {
                TempData["Message"] = "Lütfen puan geçmişinizi görmek için giriş yapın.";
                return RedirectToAction("GirisYap", "Kullanici");
            }

            try
            {
                // Kullanıcının puan geçmişini al
                var puanlar = _context.Puanlar
                    .Where(p => p.KullaniciID == kullaniciID.Value)
                    .OrderByDescending(p => p.KazanilanTarih) // Tarihe göre sıralama
                    .ToList();

                // Hiç puan kazanmadıysa bir mesaj göster
                if (!puanlar.Any())
                {
                    ViewBag.Message = "Henüz puan kazanmadınız.";
                }

                return View(puanlar);
            }
            catch (Exception ex)
            {
                // Hata durumunda mesaj döndür
                ViewBag.Message = "Puan geçmişinizi yüklerken bir hata oluştu: " + ex.Message;
                return View(new List<Puanlar>()); // Boş liste döndür
            }
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
                    kullanici.Sifre = Sifreleme.sifrele(kullanici.Sifre, "kkkk1234");

                    // İlgi alanları seçildiyse birleştir
                    var ilgiAlanlari = Request.Form["IlgiAlanlari"];
                    if (ilgiAlanlari.Count > 0)
                    {
                        // Seçilen ilgi alanlarını virgülle birleştir
                        kullanici.IlgiAlanlari = string.Join(", ", ilgiAlanlari);
                    }

                    if (Request.Form.Files.Count > 0)
                    {
                        var file = Request.Form.Files[0]; // İlk dosyayı al
                        string dosyaAdi = Path.GetFileName(file.FileName);
                        string uzanti = Path.GetExtension(file.FileName);
                        string yeniDosyaAdi = Guid.NewGuid().ToString() + uzanti; // Benzersiz bir isim oluştur
                        string yol = Path.Combine("wwwroot/Image", yeniDosyaAdi); // Dosya kaydedilecek yol

                        using (var stream = new FileStream(yol, FileMode.Create))
                        {
                            await file.CopyToAsync(stream); // Dosyayı fiziksel olarak kaydet
                        }

                        kullanici.ProfilFotografi = "/Image/" + yeniDosyaAdi; // Veri tabanına kaydedilecek yol
                    }

                    _context.Kullanicilar.Add(kullanici); // Veritabanına kaydet
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Kayıt başarılı!";
                    return RedirectToAction("KayitOl");
                }
                catch (Exception)
                {
                    TempData["Message"] = "Bir hata oluştu!";
                    return RedirectToAction("KayitOl");
                }
            }
            return View(kullanici);
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
            sifre = Sifreleme.sifrele(sifre, "kkkk1234");
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

                // Etkinlik durumunu otomatik olarak "Onay Bekliyor" şeklinde ayarla
                etkinlik.EtkinlikDurumu = "Onay Bekliyor";

                // Etkinliği veritabanına ekle
                _context.Etkinlikler.Add(etkinlik);
                await _context.SaveChangesAsync(); // Save sonrası ID atanır

                // Etkinliği oluşturan kullanıcıyı Katilimcilar tablosuna ekle
                var katilimci = new Katilimcilar
                {
                    KullaniciID = kullaniciID.Value,
                    EtkinlikID = etkinlik.ID
                };
                _context.Katilimcilar.Add(katilimci);
                await _context.SaveChangesAsync();

                // Kullanıcıya 15 puan ekle
                await PuanEkle(kullaniciID.Value, 15, "Etkinlik oluşturma");

                // Başarılı mesaj
                ViewBag.Message = "Etkinlik başarıyla oluşturuldu ve 15 puan kazandınız!";
                return RedirectToAction("KullaniciArayuz", "Kullanici");
            }
            catch (Exception ex)
            {
                // Hata durumunda kullanıcıya hata mesajı göster ve mevcut verileri geri gönder
                ViewBag.Message = "Bir hata oluştu: " + ex.Message;
                return View(etkinlik);
            }
        }




        [HttpPost]
        public async Task<IActionResult> EtkinligeKatil(int etkinlikID)
        {
            var kullaniciID = HttpContext.Session.GetInt32("KullaniciID");

            if (!kullaniciID.HasValue)
            {
                TempData["Message"] = "Kullanıcı giriş yapmamış. Lütfen giriş yapın.";
                return RedirectToAction("EtkinlikleriGoruntule");
            }

            var mevcutKatilim = _context.Katilimcilar
                .FirstOrDefault(k => k.KullaniciID == kullaniciID.Value && k.EtkinlikID == etkinlikID);

            if (mevcutKatilim != null)
            {
                TempData["Message"] = "Bu etkinliğe zaten katıldınız.";
                return RedirectToAction("EtkinlikleriGoruntule");
            }

            var etkinlik = await _context.Etkinlikler.FindAsync(etkinlikID);
            if (etkinlik == null)
            {
                TempData["Message"] = "Etkinlik bulunamadı.";
                return RedirectToAction("EtkinlikleriGoruntule");
            }

            var yeniEtkinlikBaslangic = etkinlik.Tarih.Date + etkinlik.Saat;
            var yeniEtkinlikBitis = yeniEtkinlikBaslangic + etkinlik.EtkinlikSuresi;

            var katildigiEtkinlikler = _context.Katilimcilar
                .Where(k => k.KullaniciID == kullaniciID.Value)
                .Select(k => new
                {
                    Baslangic = k.Etkinlik.Tarih.Date + k.Etkinlik.Saat,
                    Bitis = k.Etkinlik.Tarih.Date + k.Etkinlik.Saat + k.Etkinlik.EtkinlikSuresi
                })
                .ToList();

            bool cakismaVar = katildigiEtkinlikler.Any(e =>
                yeniEtkinlikBaslangic < e.Bitis && yeniEtkinlikBitis > e.Baslangic);

            if (cakismaVar)
            {
                TempData["Message"] = "Bu etkinliğe katılamazsınız, çünkü başka bir etkinlik ile çakışıyor.";
                return RedirectToAction("EtkinlikleriGoruntule");
            }

            var yeniKatilim = new Katilimcilar
            {
                KullaniciID = kullaniciID.Value,
                EtkinlikID = etkinlikID
            };

            _context.Katilimcilar.Add(yeniKatilim);
            await _context.SaveChangesAsync();

            // Kullanıcının daha önce herhangi bir etkinliğe katılıp katılmadığını kontrol et
            var dahaOnceKatildiMi = _context.Katilimcilar
                .Any(k => k.KullaniciID == kullaniciID.Value && k.EtkinlikID != etkinlikID);

            // Eğer ilk etkinlikse 20 puan, değilse 10 puan ekle
            if (!dahaOnceKatildiMi)
            {
                await PuanEkle(kullaniciID.Value, 20, "İlk etkinlik katılımı");
                TempData["Message"] = "Etkinliğe başarıyla katıldınız, 20 puan kazandınız!";
            }
            else
            {
                await PuanEkle(kullaniciID.Value, 10, "Etkinlik katılımı");
                TempData["Message"] = "Etkinliğe başarıyla katıldınız, 10 puan kazandınız!";
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
            // Etkinlik bilgilerini al
            var etkinlik = _context.Etkinlikler.FirstOrDefault(e => e.ID == etkinlikID);
            if (etkinlik == null)
            {
                TempData["ErrorMessage"] = "Etkinlik bulunamadı.";
                return RedirectToAction("KatildigimEtkinlikler", "Kullanici");
            }

            // Mesajları al
            var mesajlar = _context.Mesajlar
                .Where(m => m.EtkinlikID == etkinlikID)
                .OrderByDescending(m => m.GonderimZamani)
                .ToList();

            // Mesaj gönderen kişilerin ID'lerini al
            var gondericiIDs = mesajlar.Select(m => m.GondericiID).Distinct().ToList();

            // Kullanıcı adlarını al
            var kullaniciAdiListesi = _context.Kullanicilar
                .Where(k => gondericiIDs.Contains(k.ID))
                .ToDictionary(k => k.ID, k => k.KullaniciAdi);  // ID'yi anahtar, kullanıcı adını değer olarak alıyoruz

            // Mesajlarla birlikte kullanıcı adlarını ekle
            var mesajlarWithKullaniciAdi = mesajlar.Select(m => new MesajViewModel
            {
                MesajMetni = m.MesajMetni,
                GonderimZamani = m.GonderimZamani,
                KullaniciAdi = kullaniciAdiListesi.ContainsKey(m.GondericiID) ? kullaniciAdiListesi[m.GondericiID] : "Bilinmiyor"
            }).ToList();

            // Mesajları View'e gönder
            var model = new MesajGonderViewModel
            {
                Mesajlar = mesajlarWithKullaniciAdi,
                EtkinlikAdi = etkinlik.EtkinlikAdi,
                EtkinlikID = etkinlikID
            };

            return View(model);
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
            yeniSifre = Sifreleme.sifrele(yeniSifre, "kkkk1234");
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

            // Kullanıcı bilgilerini güncelle
            mevcutKullanici.Ad = guncelKullanici.Ad;
            mevcutKullanici.Soyad = guncelKullanici.Soyad;
            mevcutKullanici.Eposta = guncelKullanici.Eposta;
            mevcutKullanici.TelefonNumarasi = guncelKullanici.TelefonNumarasi;
            mevcutKullanici.Konum = guncelKullanici.Konum;

            // Profil fotoğrafını işlemek
            try
            {
                if (Request.Form.Files.Count > 0)
                {
                    var dosya = Request.Form.Files[0];
                    if (dosya != null && dosya.Length > 0)
                    {
                        string uzanti = Path.GetExtension(dosya.FileName);
                        string yeniDosyaAdi = Guid.NewGuid().ToString() + uzanti; // Benzersiz isim
                        string kaydetmeYolu = Path.Combine("wwwroot/Image", yeniDosyaAdi);

                        // Dosyayı kaydet
                        using (var stream = new FileStream(kaydetmeYolu, FileMode.Create))
                        {
                            await dosya.CopyToAsync(stream);
                        }

                        // Eski dosyayı sil (isteğe bağlı)
                        if (!string.IsNullOrEmpty(mevcutKullanici.ProfilFotografi))
                        {
                            string eskiDosyaYolu = Path.Combine("wwwroot", mevcutKullanici.ProfilFotografi.TrimStart('/'));
                            if (System.IO.File.Exists(eskiDosyaYolu))
                            {
                                System.IO.File.Delete(eskiDosyaYolu);
                            }
                        }

                        // Yeni profil fotoğrafı yolunu kaydet
                        mevcutKullanici.ProfilFotografi = "/Image/" + yeniDosyaAdi;
                    }
                }

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



        public async Task PuanEkle(int kullaniciID, int puan, string aciklama)
        {
            var yeniPuan = new Puanlar
            {
                KullaniciID = kullaniciID,
                KazanilanTarih = DateTime.Now,
                Puan = puan
            };

            _context.Puanlar.Add(yeniPuan);
            await _context.SaveChangesAsync();

        }

        public string GetKullaniciAdiById(int kullaniciID)
        {
            // Kullanıcıyı veritabanından al
            var kullanici = _context.Kullanicilar.FirstOrDefault(k => k.ID == kullaniciID);

            // Kullanıcı bulunamazsa boş string döndür
            if (kullanici == null)
                return "Bilinmeyen Kullanıcı";

            // Kullanıcı adı döndür
            return kullanici.KullaniciAdi;
        }


    }
}