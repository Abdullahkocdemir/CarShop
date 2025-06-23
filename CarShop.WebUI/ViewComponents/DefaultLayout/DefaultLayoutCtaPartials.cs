using DTOsLayer.WebUIDTO.CalltoActionDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarShop.WebUI.ViewComponents.DefaultLayout
{
    public class DefaultLayoutCtaPartials : ViewComponent
    {
        private readonly HttpClient _httpClient;

        public DefaultLayoutCtaPartials(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var response = await _httpClient.GetAsync("api/CalltoActions");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultCalltoActionDTO>>(jsonData);
                return View(values);
            }
            return View();
        }
    }
}
