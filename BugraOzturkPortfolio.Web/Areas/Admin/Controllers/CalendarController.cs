using BugraOzturkPortfolio.Business.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace BugraOzturkPortfolio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CalendarController : Controller
    {
        private readonly IVisitorLogService _visitorLogService;

        public CalendarController(IVisitorLogService visitorLogService)
        {
            _visitorLogService = visitorLogService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCalendarEvents()
        {
            var visitorData = await _visitorLogService.GetVisitorCountHistoryForCalendarAsync();

            var events = visitorData.Select((kvp, index) => new
            {
                id = index + 1,
                title = $"{kvp.Value} Ziyaretçi",
                date = kvp.Key,    
                category = kvp.Value > 50 ? "danger" :  
                           kvp.Value > 20 ? "warning" : 
                                            "success"   
            }).ToList();

            return Json(events);
        }
    }
}