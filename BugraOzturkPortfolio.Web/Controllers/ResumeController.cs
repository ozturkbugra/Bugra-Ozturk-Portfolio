using Microsoft.AspNetCore.Mvc;

namespace BugraOzturkPortfolio.Web.Controllers
{
    public class ResumeController : Controller
    {
        [HttpGet("ozgecmis")]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Özgeçmişim";
            return View();
        }
    }
}