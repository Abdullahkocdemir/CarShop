using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebUI.ViewComponents.AdminLayout
{
    public class _AdminLayoutSideBarPartials : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
