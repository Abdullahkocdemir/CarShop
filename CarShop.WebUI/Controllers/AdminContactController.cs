using DTOsLayer.WebUIDTO.ContactDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CarShop.WebUI.Controllers
{
    public class AdminContactController : Controller
    {
        private readonly HttpClient _httpClient;
        public AdminContactController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/Contacts");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultContactDTO>>(jsonData);
                return View(values);
            }
            ModelState.AddModelError("", "Mesajlar yüklenirken bir hata oluştu.");
            return View(new List<ResultContactDTO>());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/Contacts/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdContactDTO>(jsonData);
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan Mesaj detayları bulunamadı.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var responseMessage = await _httpClient.DeleteAsync($"api/Contacts/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Mesaj başarıyla silindi!";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan mesaj silinirken bir hata oluştu.";
            return RedirectToAction("Index");
        }
    }
}