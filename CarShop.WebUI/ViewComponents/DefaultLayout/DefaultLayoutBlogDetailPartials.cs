using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebUI.ViewComponents.DefaultLayout
{
    public class DefaultLayoutBlogDetailPartials : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
