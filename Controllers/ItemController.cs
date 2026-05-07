using Microsoft.AspNetCore.Mvc;

namespace Library_Item.Controllers
{
    public class ItemsController : Controller
    {
        public IActionResult Details(int id)
        {
            ViewBag.ItemId = id;
            return View();
        }
    }
}