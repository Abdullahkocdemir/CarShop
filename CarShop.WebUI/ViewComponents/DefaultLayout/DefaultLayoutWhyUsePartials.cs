using DTOsLayer.WebUIDTO.WhyUseDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarShop.WebUI.ViewComponents.DefaultLayout
{
    public class DefaultLayoutWhyUsePartials : ViewComponent
    {
        private readonly HttpClient _httpClient;

        public DefaultLayoutWhyUsePartials(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var response = await _httpClient.GetAsync("api/WhyUses");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultWhyUseDTO>>(jsonData);
                return View(values);
            }
            return View();
        }
    }
}
