using Microsoft.AspNetCore.Mvc;
using BugraOzturkPortfolio.Business.Abstract;

namespace BugraOzturkPortfolio.Web.ViewComponents
{
    public class ContactViewComponent : ViewComponent
    {
        private readonly IAboutService _aboutService;

        public ContactViewComponent(IAboutService aboutService)
        {
            _aboutService = aboutService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var aboutData = await _aboutService.GetAboutDataAsync();

            // Matematiksel Bot Koruması
            var rand = new System.Random();
            int num1 = rand.Next(1, 10);
            int num2 = rand.Next(1, 10);

            HttpContext.Session.SetInt32("CaptchaResult", num1 + num2);

            ViewBag.Num1 = num1;
            ViewBag.Num2 = num2;

            return View(aboutData);
        }
    }
}