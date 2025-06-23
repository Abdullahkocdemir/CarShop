using DTOsLayer.WebUIDTO.FeatureSubstancesDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarShop.WebUI.ViewComponents.DefaultLayout
{
    public class DefaultLayoutFeatureSubstancePartials : ViewComponent
    {
        private readonly HttpClient _httpClient;

        public DefaultLayoutFeatureSubstancePartials(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var response = await _httpClient.GetAsync("api/FeatureSubstances");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultFeatureSubstancesDTO>>(jsonData);
                return View(values);
            }
            return View();
        }
    }
}
