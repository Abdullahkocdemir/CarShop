using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebUI.Controllers
{
    public class _AdminLayoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
