using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebUI.ViewComponents.AdminLayout
{
    public class _AdminLayoutHeaderPartials:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
