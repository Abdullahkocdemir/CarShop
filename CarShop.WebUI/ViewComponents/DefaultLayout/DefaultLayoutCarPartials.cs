using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebUI.ViewComponents.DefaultLayout
{
    public class DefaultLayoutCarPartials : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
