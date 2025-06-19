using DTOsLayer.WebUIDTO.WhyUseDTO;
using DTOsLayer.WebUIDTO.WhyUseReasonDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Linq; // LINQ metotları için ekleyin

namespace CarShop.WebUI.Controllers
{
    public class WhyUseController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://localhost:7234/api/WhyUses"; // API'nin çalıştığı URL'yi buraya yazmalısın

        public WhyUseController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(); // Named Client kullanmak yerine varsayılan client'ı kullanıyoruz veya buraya "CarShopApiClient" gibi özel bir client tanımı yapabilirsin. Eğer özel bir client kullanıyorsan, program.cs'de tanımlaman gerekir.
            _httpClient.BaseAddress = new Uri(BaseUrl);
        }

        // --- Listeleme (READ) ---
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var responseMessage = await _httpClient.GetAsync(BaseUrl);
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultWhyUseUIDTO>>(jsonData);
                return View(values);
            }
            return View(); // Hata durumunda boş bir view döndürebiliriz
        }

        // --- Yeni Ekleme (CREATE) ---
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateWhyUseDTO createWhyUseDto)
        {
            if (!ModelState.IsValid)
            {
                return View(createWhyUseDto);
            }

            var jsonContent = JsonConvert.SerializeObject(createWhyUseDto);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var responseMessage = await _httpClient.PostAsync(BaseUrl, content);

            if (responseMessage.IsSuccessStatusCode)
            {
                // Başarılı olursa listeleme sayfasına yönlendir
                return RedirectToAction("Index");
            }
            // API'den gelen hata mesajını göstermek için yakalayabiliriz
            var errorContent = await responseMessage.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Kayıt eklenirken bir hata oluştu: {errorContent}");
            return View(createWhyUseDto);
        }

        // --- Güncelleme (UPDATE) ---
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var responseMessage = await _httpClient.GetAsync($"{BaseUrl}/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var jsonData = await responseMessage.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<UpdateWhyUseDTO>(jsonData);
                return View(value);
            }
            return NotFound(); // Kayıt bulunamazsa 404
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateWhyUseDTO updateWhyUseDto)
        {
            if (!ModelState.IsValid)
            {
                return View(updateWhyUseDto);
            }

            var jsonContent = JsonConvert.SerializeObject(updateWhyUseDto);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var responseMessage = await _httpClient.PutAsync(BaseUrl, content);

            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            var errorContent = await responseMessage.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Kayıt güncellenirken bir hata oluştu: {errorContent}");
            return View(updateWhyUseDto);
        }

        // --- Silme (DELETE) ---
        public async Task<IActionResult> Delete(int id)
        {
            var responseMessage = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            // Hata durumunda ne yapılacağına karar verilebilir, örneğin bir hata mesajı gösterebiliriz.
            var errorContent = await responseMessage.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"Kayıt silinirken bir hata oluştu: {errorContent}";
            return RedirectToAction("Index");
        }
    }
}