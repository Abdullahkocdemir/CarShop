using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebUI.ViewComponents.DefaultLayout
{
    public class DefaultLayoutContactPartials : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
