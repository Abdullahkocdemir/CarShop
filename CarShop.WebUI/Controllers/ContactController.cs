using DTOsLayer.WebUIDTO.ContactDTO; // WebUI DTO'larını kullanıyoruz
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations; // Validasyon için

namespace CarShop.WebUI.Controllers
{
    public class ContactController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "api/Contacts"; // API Controller'ımızın doğru endpoint'i

        public ContactController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
        }

        // İletişim formunu görüntüleyen Index metodu
        public IActionResult Index()
        {
            // View'e boş bir CreateContactDTO gönderiyoruz ki form doğru çalışsın
            return View(new CreateContactDTO());
        }

        // İletişim formunun gönderildiği metot
        [HttpPost]
        [ValidateAntiForgeryToken] // CSRF saldırılarına karşı koruma
        public async Task<IActionResult> SubmitContactForm([FromForm] CreateContactDTO dto) // Formdan gelen veriyi yakalamak için [FromForm] eklenebilir
        {
            // Model validasyonunu kontrol et
            if (!ModelState.IsValid)
            {
                // Eğer validasyon hatası varsa, aynı DTO ile view'i tekrar göster
                return View("Index", dto);
            }

            try
            {
                // DTO'yu JSON'a dönüştürürken camelCase formatını kullan
                var jsonContent = JsonSerializer.Serialize(dto, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase // API'nin beklediği format genellikle camelCase'dir
                });

                // JSON içeriği ve medya tipini belirt
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // API'ye POST isteği gönder (Doğru endpoint'i kullan: api/Contacts)
                var response = await _httpClient.PostAsync(ApiBaseUrl, content);

                // Eğer istek başarılıysa
                if (response.IsSuccessStatusCode)
                {
                    // Başarı mesajını TempData'ya kaydet ve Index sayfasına yönlendir
                    TempData["SuccessMessage"] = "Your message has been sent successfully! We will get back to you soon.";
                    return RedirectToAction("Index");
                }
                else // İstek başarısızsa (API'den hata döndüyse)
                {
                    var error = await response.Content.ReadAsStringAsync(); // API'den gelen hata mesajını oku
                    // ModelState'e genel bir hata ekle
                    ModelState.AddModelError("", $"API Error: {response.StatusCode} - {error}");
                    // Hatalı DTO ile view'i tekrar göster
                    return View("Index", dto);
                }
            }
            catch (HttpRequestException ex) // HTTP isteği sırasında oluşan bağlantı veya ağ hataları
            {
                ModelState.AddModelError("", $"Connection error: Could not reach the API. Please try again later. Details: {ex.Message}");
                return View("Index", dto);
            }
            catch (Exception ex) // Beklenmedik diğer hatalar
            {
                ModelState.AddModelError("", $"An unexpected error occurred: {ex.Message}");
                return View("Index", dto);
            }
        }
    }
}
