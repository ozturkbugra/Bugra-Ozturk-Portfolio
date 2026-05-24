using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.DataAccess.Repositories.Abstract;
using BugraOzturkPortfolio.Entities.Concrete;
using Microsoft.AspNetCore.Authorization;

namespace BugraOzturkPortfolio.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var totalProjects = await _unitOfWork.GetRepository<Project>().CountAsync();
            ViewBag.TotalProjects = totalProjects;

            return View();
        }
    }
}