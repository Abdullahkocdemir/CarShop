using DTOsLayer.WebUIDTO.BlogDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarShop.WebUI.Controllers
{
    public class BlogController : Controller
    {
        private readonly HttpClient _httpClient;

        public BlogController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/Blogs");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultBlogDTO>>(jsonData);
                return View(values);
            }
            return View();
        }
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/Blogs/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdBlogDTO>(jsonData);
                return View(value);
            }
            return View();
        }
    }
}
