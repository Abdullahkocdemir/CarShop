using DTOsLayer.WebUIDTO.BroadcastDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarShop.WebUI.Controllers
{
    public class BroadCastController : Controller
    {
        private readonly HttpClient _httpClient;

        public BroadCastController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/Broadcasts");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultBroadcastDTO>>(jsonData);
                return View(values);
            }
            return View();
        }
    }
}
