using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Business.Abstract;
using System.Threading.Tasks;

namespace BugraOzturkPortfolio.Web.Controllers
{
    public class ServicesController : Controller
    {
        [HttpGet("hizmetler")]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Hizmetlerim";
            return View();
        }
    }
}