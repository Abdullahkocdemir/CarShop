using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebUI.ViewComponents.DefaultLayout
{
    public class DefaultLayoutContactTitlePartials : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
