using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebUI.ViewComponents.AdminLayout
{
    public class _AdminLayoutFooterPartials : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
