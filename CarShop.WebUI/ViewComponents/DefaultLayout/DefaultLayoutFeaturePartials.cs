using DTOsLayer.WebUIDTO.FeatureDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarShop.WebUI.ViewComponents.DefaultLayout
{
    public class DefaultLayoutFeaturePartials : ViewComponent
    {
        private readonly HttpClient _httpClient;

        public DefaultLayoutFeaturePartials(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var response = await _httpClient.GetAsync("api/Features");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultFeatureDTO>>(jsonData);
                return View(values);
            }
            return View();
        }
    }
}
