using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace GovITHub.Auth.Identity.Controllers
{
    public class HomeController : Controller
    {
        IStringLocalizer<HomeController> stringLocalizer;
        ILogger<HomeController> logger;
        public HomeController(IStringLocalizer<HomeController> stringLocalizer, ILogger<HomeController> logger)
        {
            this.stringLocalizer = stringLocalizer;
            this.logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = stringLocalizer["Your application description page."];

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = stringLocalizer["Your contact page."];

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        [Authorize]
        public IActionResult Localization()
        {
            return View();
        }
    }
}
