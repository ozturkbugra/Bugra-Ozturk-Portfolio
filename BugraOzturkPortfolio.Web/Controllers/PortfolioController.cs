using Microsoft.AspNetCore.Mvc;

namespace BugraOzturkPortfolio.Web.Controllers
{
    public class PortfolioController : Controller
    {
        [HttpGet("portfolyo")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Portfolyom";
            return View();
        }
    }
}
