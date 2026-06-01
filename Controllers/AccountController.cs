using Library_Item.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Library_Item.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // =======================================================
        // GERÇEK GİRİŞ YAPMA İŞLEMLERİ (LOGIN)
        // =======================================================

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email && u.Password == Password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.GivenName, user.Username),
                    new Claim(ClaimTypes.Role, "User"),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            ViewBag.HataMesaji = "E-posta veya şifre hatalı!";
            return View();
        }

        // =======================================================
        // GERÇEK KAYIT OLMA İŞLEMLERİ (REGISTER)
        // =======================================================

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string Email, string Password, string Username)
        {
            var varMi = await _context.Users.AnyAsync(u => u.Email == Email);
            if (varMi)
            {
                ViewBag.HataMesaji = "Bu e-posta adresi zaten kullanımda!";
                return View();
            }

            User yeniKullanici = new User
            {
                Email = Email,
                Password = Password,
                Username = Username
            };

            _context.Users.Add(yeniKullanici);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        // =======================================================
        // GÜVENLİ ÇIKIŞ YAPMA İŞLEMİ (LOGOUT)
        // =======================================================

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // =======================================================
        // KORUMALI İLAN İŞLEMLERİ (CRUD)
        // =======================================================
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(Item ilan)
        {
            // 🚨 Giriş yapan kullanıcının ID'sini kimlik kartından (Claim) okuyoruz
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userIdString))
            {
                ilan.UserId = int.Parse(userIdString); // İlanın sahibini belirliyoruz
            }

            if (ilan.ImageFile != null && ilan.ImageFile.Length > 0)
            {
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ilan.ImageFile.FileName);
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ilan.ImageFile.CopyToAsync(fileStream);
                }

                ilan.ImageUrl = "/images/" + uniqueFileName;
            }
            else
            {
                ilan.ImageUrl = "https://images.unsplash.com/photo-1531403009284-440f080d1e12?w=500&auto=format&fit=crop";
            }

            _context.Items.Add(ilan);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var ilan = await _context.Items.FindAsync(id);

            if (ilan != null)
            {
                _context.Items.Remove(ilan);
                await _context.SaveChangesAsync();
                TempData["Mesaj"] = "İlan başarıyla silindi.";
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var ilan = await _context.Items.FindAsync(id);
            if (ilan == null)
            {
                return NotFound();
            }
            return View(ilan);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(Item guncelIlan)
        {
            if (guncelIlan != null)
            {
                var eskiIlan = await _context.Items.FindAsync(guncelIlan.Id);

                if (eskiIlan != null)
                {
                    eskiIlan.Title = guncelIlan.Title;
                    eskiIlan.Description = guncelIlan.Description;
                    eskiIlan.Price = guncelIlan.Price;
                    eskiIlan.Category = guncelIlan.Category;

                    if (guncelIlan.ImageFile != null && guncelIlan.ImageFile.Length > 0)
                    {
                        string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(guncelIlan.ImageFile.FileName);
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await guncelIlan.ImageFile.CopyToAsync(fileStream);
                        }
                        eskiIlan.ImageUrl = "/images/" + uniqueFileName;
                    }

                    _context.Items.Update(eskiIlan);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "Home");
                }
            }
            return View(guncelIlan);
        }

        // =======================================================
        // SEPETE ÜRÜN EKLEME METODU (POST)
        // =======================================================
        // =======================================================
        // SEPETE ÜRÜN EKLEME (GÜNCELLENDİ: Telefon Numarasını da Hafızaya Alıyoruz)
        // =======================================================
        [HttpPost]
        public async Task<IActionResult> AddToBasket(int itemId, string customerPhone)
        {
            var item = await _context.Items.FindAsync(itemId);
            if (item == null) return NotFound();

            // Ürün ID'sini ve girilen telefon numarasını hafızaya kaydediyoruz
            HttpContext.Session.SetInt32("SelectedItemId", itemId);
            HttpContext.Session.SetString("CustomerPhone", customerPhone ?? "");

            return RedirectToAction("Basket");
        }

        // =======================================================
        // SEPETİ LİSTELEME (GÜNCELLENDİ: Telefon Numarasını Sayfaya Taşıyoruz)
        // =======================================================
        [HttpGet]
        public async Task<IActionResult> Basket()
        {
            int? itemId = HttpContext.Session.GetInt32("SelectedItemId");
            string phone = HttpContext.Session.GetString("CustomerPhone");

            Item sepetUrün = null;

            if (itemId.HasValue)
            {
                sepetUrün = await _context.Items.FindAsync(itemId.Value);
            }

            // Telefon numarasını sepet sayfasında göstermek için ViewBag ile taşıyoruz
            ViewBag.CustomerPhone = phone;
            return View(sepetUrün);
        }

        // =======================================================
        // SEPETİ ONAYLAMA METODU (POST)
        // =======================================================
        [HttpPost]
        public IActionResult ConfirmOrder()
        {
            // Onay işleminden sonra sepeti (hafızayı) temizliyoruz
            HttpContext.Session.Remove("SelectedItemId");
            HttpContext.Session.Remove("CustomerPhone");

            // Kullanıcıyı onay alındı sayfasına yönlendiriyoruz
            return RedirectToAction("OrderSuccess");
        }

        // =======================================================
        // ONAYLANDI / BAŞARILI SAYFASI (GET)
        // =======================================================
        [HttpGet]
        public IActionResult OrderSuccess()
        {
            return View();
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyItems()
        {
            // 1. Giriş yapan kullanıcının benzersiz ID'sini alıyoruz
            var userIdString = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return RedirectToAction("Login");
            }

            int girenKullaniciId = int.Parse(userIdString);

            // 2. Veri tabanından SADECE bu kullanıcının eklediği ilanları getiriyoruz
            var kullaniciIlanlari = await _context.Items
                                                 .Where(i => i.UserId == girenKullaniciId)
                                                 .ToListAsync();

            // 3. Bu ilanları oluşturduğumuz MyItems.cshtml sayfasına gönderiyoruz
            return View(kullaniciIlanlari);
        }
        // 1. Sayfayı İlk Kez Açan Kod (Formu Gösterir)
        [Authorize]
        [HttpGet]
        public IActionResult AddItem()
        {
            return View();
        }

        // 2. Form Doldurulup "Paylaş" Butonuna Basılınca Çalışan Kod (Veri Tabanına Kaydeder)
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddItem(Library_Item.Models.Item yeniUrun)
        {
            // Giriş yapan kullanıcının ID'sini ilana bağlıyoruz
            var userIdString = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userIdString))
            {
                yeniUrun.UserId = int.Parse(userIdString);
            }

            // Kullanıcı bir görsel dosyası yüklediyse onu sunucuya kaydediyoruz
            if (yeniUrun.ImageFile != null && yeniUrun.ImageFile.Length > 0)
            {
                // Benzersiz bir dosya adı oluşturuyoruz (aynı isimli dosyaların çakışmasını engeller)
                string benzersizAd = Guid.NewGuid().ToString() + Path.GetExtension(yeniUrun.ImageFile.FileName);

                // Dosyanın kaydedileceği klasör yolu: wwwroot/images/
                string yuklemeklasoru = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                // Eğer images klasörü yoksa oluşturuyoruz
                if (!Directory.Exists(yuklemeklasoru))
                {
                    Directory.CreateDirectory(yuklemeklasoru);
                }

                string dosyaYolu = Path.Combine(yuklemeklasoru, benzersizAd);

                // Dosyayı sunucudaki klasöre kaydediyoruz
                using (var stream = new FileStream(dosyaYolu, FileMode.Create))
                {
                    await yeniUrun.ImageFile.CopyToAsync(stream);
                }

                // Veri tabanına kaydedilecek göreceli yol
                yeniUrun.ImageUrl = "/images/" + benzersizAd;
            }
            else
            {
                // Eğer kullanıcı görsel yüklemediyse varsayılan bir görsel atıyoruz
                yeniUrun.ImageUrl = "https://images.unsplash.com/photo-1531403009284-440f080d1e12?w=500&auto=format&fit=crop";
            }

            // Ürün bilgilerini veri tabanına ekliyoruz
            _context.Items.Add(yeniUrun);
            await _context.SaveChangesAsync();

            // İlan eklenince kullanıcının kendi ilanları sayfasına geri gönderiyoruz
            return RedirectToAction("MyItems");
        }
    }
}