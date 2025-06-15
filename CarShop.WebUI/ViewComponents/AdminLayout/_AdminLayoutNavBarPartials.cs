using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebUI.ViewComponents.AdminLayout
{
    public class _AdminLayoutNavBarPartials : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
