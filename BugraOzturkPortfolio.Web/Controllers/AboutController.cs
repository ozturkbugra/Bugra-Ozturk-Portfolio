using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Business.Abstract;

namespace BugraOzturkPortfolio.Web.Controllers
{
    public class AboutController : Controller
    {
        [HttpGet("hakkimda")]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Hakkımda";
            return View();
        }
    }
}