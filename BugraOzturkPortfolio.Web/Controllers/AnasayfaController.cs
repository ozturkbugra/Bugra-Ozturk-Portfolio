using Microsoft.AspNetCore.Mvc;

namespace BugraOzturkPortfolio.Web.Controllers
{
    public class AnasayfaController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Anasayfa";
            return View();
        }
    }
}
