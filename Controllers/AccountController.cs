using Microsoft.AspNetCore.Mvc;

namespace Library_Item.Controllers
{
    public class AccountController : Controller
    {

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string Email, string Password)
        {
           
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(string Email, string Password, string Username)
        {
            
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Basket()
        {
            return View();
        }
    }
    
}