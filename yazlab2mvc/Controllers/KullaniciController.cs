using Microsoft.AspNetCore.Mvc;
using yazlab2mvc.Models;
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
        public IActionResult Create(Kullanicilar kullanici)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Kullanicilar.Add(kullanici);
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
