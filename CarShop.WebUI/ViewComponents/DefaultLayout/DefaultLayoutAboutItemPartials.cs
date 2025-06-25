using DTOsLayer.WebUIDTO.AboutItemDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarShop.WebUI.ViewComponents.DefaultLayout
{
    public class DefaultLayoutAboutItemPartials : ViewComponent
    {
        private readonly HttpClient _httpClient;

        public DefaultLayoutAboutItemPartials(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var response = await _httpClient.GetAsync("api/AboutItems");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultAboutItemDTO>>(jsonData);
                return View(values);
            }
            return View();
        }
    }
}
