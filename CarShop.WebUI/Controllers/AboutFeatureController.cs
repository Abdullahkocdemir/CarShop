using DTOsLayer.WebUIDTO.AboutFeatureDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using System.Net.Http;

namespace CarShop.WebUI.Controllers
{
    public class AboutFeatureController : Controller
    {
        private readonly HttpClient _httpClient;

        public AboutFeatureController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
        }


        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/AboutFeatures");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultAboutFeatureDTO>>(jsonData);
                return View(values);
            }
            return View(new List<ResultAboutFeatureDTO>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAboutFeatureDTO dto)
        {
            if (ModelState.IsValid)
            {
                using var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(dto.Title), "Title");
                formData.Add(new StringContent(dto.Description), "Description");

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

                var response = await _httpClient.PostAsync("api/AboutFeatures", formData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Hakkımızda Özelliği başarıyla eklendi!";
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
            var response = await _httpClient.GetAsync($"api/AboutFeatures/{id}");
            if (response.IsSuccessStatusCode)
            {
                var apiDto = JsonConvert.DeserializeObject<GetByIdAboutFeatureDTO>(await response.Content.ReadAsStringAsync());

                var updateDto = new UpdateAboutFeatureDTO
                {
                    AboutFeatureId = apiDto!.AboutFeatureId,
                    Title = apiDto.Title,
                    Description = apiDto.Description,
                    ExistingImageUrl = apiDto.ImageUrl
                };
                return View(updateDto);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan Hakkımızda Özelliği bulunamadı.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateAboutFeatureDTO dto)
        {
            if (ModelState.IsValid)
            {
                using var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(dto.AboutFeatureId.ToString()), "AboutFeatureId");
                formData.Add(new StringContent(dto.Title), "Title");
                formData.Add(new StringContent(dto.Description), "Description");

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

                var response = await _httpClient.PutAsync("api/AboutFeatures", formData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Hakkımızda Özelliği başarıyla güncellendi!";
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
            var responseMessage = await _httpClient.DeleteAsync($"api/AboutFeatures/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Hakkımızda Özelliği başarıyla silindi!";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan Hakkımızda Özelliği silinirken bir hata oluştu.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/AboutFeatures/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdAboutFeatureDTO>(jsonData);
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan Hakkımızda Özelliği detayları bulunamadı.";
            return RedirectToAction("Index");
        }
    }
}