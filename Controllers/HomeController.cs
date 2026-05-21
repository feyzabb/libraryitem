using Microsoft.AspNetCore.Mvc;
using Library_Item.Models;
using System.Linq;

namespace Library_Item.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        // Veri tabanýný tanýyan constructor
        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        // Ana sayfa ve kategorilerin can damarý olan metot
        public IActionResult Index(string category, string searchString)
        {
            // Veri tabanýndaki ilanlarý çekiyoruz
            var ilanlar = _context.Items.AsQueryable();

            // Eðer üstten veya soldan bir kategoriye týklandýysa filtrele
            if (!string.IsNullOrEmpty(category))
            {
                ilanlar = ilanlar.Where(x => x.Category == category);
                ViewBag.SelectedCategory = category;
            }

            // Eðer arama kutusuna bir þey yazýldýysa filtrele
            if (!string.IsNullOrEmpty(searchString))
            {
                ilanlar = ilanlar.Where(s => s.Title.Contains(searchString) || s.Description.Contains(searchString));
                ViewData["CurrentFilter"] = searchString;
            }

            // Gitarlarý ve kemanlarý HTML sayfana paketleyip gönderiyoruz
            return View(ilanlar.ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}