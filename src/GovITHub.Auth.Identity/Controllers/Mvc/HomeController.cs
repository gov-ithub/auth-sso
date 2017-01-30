using Localization.SqlLocalizer.DbStringLocalizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace GovITHub.Auth.Identity.Controllers
{
    public class HomeController : Controller
    {
        IStringLocalizer<HomeController> _stringLocalizer;
        ILogger<HomeController> _logger;
        IStringExtendedLocalizerFactory _stringLocalizerFactory;
        public HomeController(IStringLocalizer<HomeController> stringLocalizer, ILogger<HomeController> logger, IStringExtendedLocalizerFactory stringLocalizerFactory)
        {
            _stringLocalizer = stringLocalizer;
            _logger = logger;
            _stringLocalizerFactory = stringLocalizerFactory;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = _stringLocalizer["Your application description page."];

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = _stringLocalizer["Your contact page."];

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
        [Authorize]
        public IActionResult LocalizationReset()
        {
            _stringLocalizerFactory.ResetCache();
            return RedirectToAction("Localization");
        }
    }
}
