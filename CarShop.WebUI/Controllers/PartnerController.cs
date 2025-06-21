using DTOsLayer.WebUIDTO.PartnerDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using System.Net.Http;

namespace CarShop.WebUI.Controllers
{
    public class PartnerController : Controller
    {
        private readonly HttpClient _httpClient;

        public PartnerController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
        }


        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/Partners");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultPartnerDTO>>(jsonData);
                return View(values);
            }
            return View(new List<ResultPartnerDTO>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePartnerDTO dto)
        {
            if (ModelState.IsValid)
            {
                using var formData = new MultipartFormDataContent();

                if (dto.ImageFile != null && dto.ImageFile.Length > 0)
                {
                    var fileContent = new StreamContent(dto.ImageFile.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(dto.ImageFile.ContentType);
                    formData.Add(fileContent, "ImageFile", dto.ImageFile.FileName);
                }
                else
                {
                    ModelState.AddModelError("ImageFile", "Lütfen bir resim dosyası yükleyin.");
                    return View(dto);
                }

                var response = await _httpClient.PostAsync("api/Partners", formData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Ortak başarıyla eklendi!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"API Hatası: {response.StatusCode} - {errorContent}");
                }
            }
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"api/Partners/{id}");
            if (response.IsSuccessStatusCode)
            {
                var apiDto = JsonConvert.DeserializeObject<GetByIdPartnerDTO>(await response.Content.ReadAsStringAsync());

                var updateDto = new UpdatePartnerDTO
                {
                    PartnerId = apiDto!.PartnerId,
                    ExistingImageUrl = apiDto.ImageUrl
                };
                return View(updateDto);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan ortak bulunamadı.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdatePartnerDTO dto)
        {
            if (ModelState.IsValid)
            {
                using var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(dto.PartnerId.ToString()), "PartnerId");

                if (!string.IsNullOrEmpty(dto.ExistingImageUrl))
                {
                    formData.Add(new StringContent(dto.ExistingImageUrl), "ExistingImageUrl");
                }

                if (dto.ImageFile != null && dto.ImageFile.Length > 0)
                {
                    var fileContent = new StreamContent(dto.ImageFile.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(dto.ImageFile.ContentType);
                    formData.Add(fileContent, "ImageFile", dto.ImageFile.FileName);
                }

                var response = await _httpClient.PutAsync("api/Partners", formData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Ortak başarıyla güncellendi!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"API Hatası: {response.StatusCode} - {errorContent}");
                }
            }
            return View(dto);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var responseMessage = await _httpClient.DeleteAsync($"api/Partners/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Ortak başarıyla silindi!";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan ortak silinirken bir hata oluştu.";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/Partners/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdPartnerDTO>(jsonData);
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan ortak detayları bulunamadı.";
            return RedirectToAction("Index");
        }
    }
}