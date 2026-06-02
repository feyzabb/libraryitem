using Microsoft.AspNetCore.Mvc;
using Library_Item.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Library_Item.Controllers
{
    public class ItemsController : Controller
    {
        private readonly AppDbContext _context;

        // Veri tabanı bağlantısını buraya da güvenle tanıtıyoruz
        public ItemsController(AppDbContext context)
        {
            _context = context;
        }

        // SENİN ESKİ ÇALIŞAN DETAY METODUN (Aynen korundu)
        public IActionResult Details(int id)
        {
            // Veri tabanından tıklanan ID'ye ait ürünü buluyoruz
            var urun = _context.Items.FirstOrDefault(x => x.Id == id);

            if (urun == null)
            {
                return NotFound(); // Eğer öyle bir ürün yoksa hata sayfasına gönderir
            }

            // Bulduğumuz ürünü doğrudan View'a model olarak paketleyip gönderiyoruz
            return View(urun);
        }

        // KİRALAMA BUTONUNA BASINCA ÇALIŞACAK GÜVENLİ METOT
        [HttpPost]
        public IActionResult CreateRentalRequest(int itemId, string customerPhone)
        {
            // 1. Önce kiralama yapılan ürünün başlığını (Title) bulabilmek için id'ye göre ürünü buluyoruz
            var urun = _context.Items.FirstOrDefault(x => x.Id == itemId);

            // Eğer veri tabanında elle girilmiş id'lerden dolayı uyuşmazlık olursa hata vermesin diye varsayılan bir başlık atayalım
            string urunBasligi = urun != null ? urun.Title : "Kiralık Ürün";

            // 2. Yeni kiralama isteğini oluştururken veri tabanının zorunlu tuttuğu ItemTitle alanını da dolduruyoruz
            var yeniIstek = new RentalRequest
            {
                ItemId = itemId,
                CustomerPhone = customerPhone,
                ItemTitle = urunBasligi // VERİ TABANININ İSTEDİĞİ KRİTİK ALAN
            };

            // 3. Artık her şey tam olduğuna göre güvenle ekleyip kaydedebiliriz
            _context.RentalRequests.Add(yeniIstek);
            _context.SaveChanges();

            // İşlem bitince başarı sayfasına yönlendirir
            return RedirectToAction("RentalSuccess");
        }
        // İlanın kime ait olduğunu bilmemiz için bu alanı ekliyoruz
        public int UserId { get; set; }
        // Yeni açacağımız sayfanın metodu
        public IActionResult RentalSuccess()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public IActionResult Create(Item yeniUrun)
        {
            // Giriş yapan kullanıcının ID'sini ilana bağlıyoruz
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userIdString))
            {
                yeniUrun.UserId = int.Parse(userIdString);
            }

            if (yeniUrun.ImageFile != null)
            {
                // Benzersiz bir dosya adı oluşturuyoruz (örn: f47ac10b-resim.jpg)
                string benzersizAd = Guid.NewGuid().ToString() + "_" + yeniUrun.ImageFile.FileName;

                // Dosyanın kaydedileceği klasör yolu: wwwroot/images/
                string klasorYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", benzersizAd);

                // Dosyayı klasöre kaydediyoruz
                using (var stream = new FileStream(klasorYolu, FileMode.Create))
                {
                    yeniUrun.ImageFile.CopyTo(stream);
                }

                // Veri tabanına kaydedilecek yol
                yeniUrun.ImageUrl = "/images/" + benzersizAd;
            }
            else
            {
                // Varsayılan görsel
                yeniUrun.ImageUrl = "https://images.unsplash.com/photo-1531403009284-440f080d1e12?w=500&auto=format&fit=crop";
            }

            // ModelState'teki UserId alanını doğrulama dışı bırakıyoruz çünkü elle atadık
            ModelState.Remove("UserId");

            // Eğer formdan gelen veriler model kurallarına uygunsa
            if (ModelState.IsValid)
            {
                // Veri tabanına yeni ürünü ekliyoruz
                _context.Items.Add(yeniUrun);

                // Değişiklikleri veri tabanına kesin olarak kaydediyoruz
                _context.SaveChanges();

                // Ürün başarıyla eklendikten sonra kullanıcıyı ana sayfaya yönlendiriyoruz
                return RedirectToAction("Index", "Home");
            }

            // Eğer bir hata oluşursa yine ana sayfaya geri fırlatıyoruz
            return RedirectToAction("Index", "Home");
        }
    }
}