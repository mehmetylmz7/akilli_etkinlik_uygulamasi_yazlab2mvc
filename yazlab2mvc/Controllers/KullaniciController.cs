using Microsoft.AspNetCore.Mvc;
using yazlab2mvc.Models;
using yazlab2mvc.ViewModels;
using System.Linq;

namespace yazlab2mvc.Controllers
{
    public class KullaniciController : Controller
    {
        private readonly Context _context;

        public KullaniciController(Context context)
        {
            _context = context;
        }

        // GET: Kullanici/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Kullanici/Create
        [HttpPost]
        public IActionResult Create(CreateKullaniciViewModel kullanici)
        {


            if (ModelState.IsValid)
            {
                Kullanicilar kullaniciEntity = new Kullanicilar
                {
                    KullaniciAdi = kullanici.KullaniciAdi,
                    Sifre = kullanici.Sifre,
                    Eposta = kullanici.Eposta,
                    TelefonNumarasi = kullanici.TelefonNumarasi,
                    Ad = kullanici.Ad,
                    Soyad = kullanici.Soyad,
                    DogumTarihi = kullanici.DogumTarihi,
                    Cinsiyet = kullanici.Cinsiyet,
                    Konum = kullanici.Konum,
                    IlgiAlanlari = kullanici.IlgiAlanlari,
                    ProfilFotografi = kullanici.ProfilFotografi,
                };

                try
                {
                    _context.Kullanicilar.Add(kullaniciEntity);
                    _context.SaveChanges();
                    ViewBag.Mesaj = "Kayıt başarılı";
                    return View();
                }
                catch (Exception ex)
                {
                    ViewBag.Mesaj = "Kaydedilmedi: " + ex.Message;
                }
            }
            else
            {
                ViewBag.Mesaj = "Kaydedilmedi: Geçersiz veri";
            }
            return View(kullanici);
        }



        // GET: Kullanici/Index
        public IActionResult Index()
        {
            var kullanicilar = _context.Kullanicilar.ToList();
            return View(kullanicilar);
        }
    }
}
