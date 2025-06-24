using DTOsLayer.WebUIDTO.ShowroomDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarShop.WebUI.ViewComponents.DefaultLayout
{
    public class DefaultLayoutShowroomPartials : ViewComponent
    {
        private readonly HttpClient _httpClient;

        public DefaultLayoutShowroomPartials(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var response = await _httpClient.GetAsync("api/Showroomss");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultShowroomDTO>>(jsonData);
                return View(values);
            }
            return View();
        }
    }
}
