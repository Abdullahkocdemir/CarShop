using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebUI.ViewComponents.DefaultLayout
{
    public class DefaultLayoutCarDetailPartials : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
