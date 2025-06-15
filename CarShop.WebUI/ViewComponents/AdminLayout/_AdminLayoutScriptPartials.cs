using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebUI.ViewComponents.AdminLayout
{
    public class _AdminLayoutScriptPartials : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
