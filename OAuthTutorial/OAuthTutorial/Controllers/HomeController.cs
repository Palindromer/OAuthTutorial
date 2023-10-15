using Microsoft.AspNetCore.Mvc;

namespace OAuthTutorial.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
