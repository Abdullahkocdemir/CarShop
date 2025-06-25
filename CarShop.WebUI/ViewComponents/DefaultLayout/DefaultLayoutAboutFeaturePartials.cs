using DTOsLayer.WebUIDTO.AboutFeatureDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarShop.WebUI.ViewComponents.DefaultLayout
{
    public class DefaultLayoutAboutFeaturePartials : ViewComponent
    {
        private readonly HttpClient _httpClient;

        public DefaultLayoutAboutFeaturePartials(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var response = await _httpClient.GetAsync("api/AboutFeatures");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultAboutFeatureDTO>>(jsonData);
                return View(values);
            }
            return View();
        }
    }
}
