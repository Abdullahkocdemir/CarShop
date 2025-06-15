using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebUI.Controllers
{
    public class DefaultController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
