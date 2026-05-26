using BugraOzturkPortfolio.Business.Abstract;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace BugraOzturkPortfolio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly IVisitorLogService _visitorLogService;
        private readonly IContactMessageService _contactMessageService;
        private readonly IProjectService _projectService;

        public DashboardController(
            IVisitorLogService visitorLogService,
            IContactMessageService contactMessageService,
            IProjectService projectService)
        {
            _visitorLogService = visitorLogService;
            _contactMessageService = contactMessageService;
            _projectService = projectService;
        }

        public async Task<IActionResult> Index()
        {
            // --- ÜST KART METRİKLERİ ---
            var messages = await _contactMessageService.GetAllMessagesAsync();
            ViewBag.UnreadMessagesCount = messages.Count(x => !x.IsRead && !x.IsDeleted);
            ViewBag.TotalMessagesCount = messages.Count(x => !x.IsDeleted);

            var projects = await _projectService.GetAllProjectsAsync();
            ViewBag.TotalProjectsCount = projects.Count(x => !x.IsDeleted);

            ViewBag.DailyVisitors = await _visitorLogService.GetPeriodicVisitorsCountAsync("daily");
            ViewBag.WeeklyVisitors = await _visitorLogService.GetPeriodicVisitorsCountAsync("weekly");
            ViewBag.MonthlyVisitors = await _visitorLogService.GetPeriodicVisitorsCountAsync("monthly");
            ViewBag.YearlyVisitors = await _visitorLogService.GetPeriodicVisitorsCountAsync("yearly");

            var weekHistory = await _visitorLogService.GetLastWeekVisitorHistoryAsync();
            ViewBag.WeekHistoryLabels = string.Join(",", weekHistory.Keys.Select(k => $"'{k}'"));
            ViewBag.WeekHistoryValues = string.Join(",", weekHistory.Values);

            var topDays = await _visitorLogService.GetTopFiveMostVisitedDaysAsync();
            ViewBag.TopDaysLabels = string.Join(",", topDays.Keys.Select(k => $"'{k}'"));
            ViewBag.TopDaysValues = string.Join(",", topDays.Values);

            return View();
        }
    }
}