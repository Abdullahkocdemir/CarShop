using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using DTOsLayer.WebUIDTO.FeatureDTO;
using System.IO;

namespace CarShop.WebUI.Controllers
{
    public class FeatureController : Controller
    {
        private readonly HttpClient _httpClient;

        public FeatureController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/Features");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultFeatureDTO>>(jsonData);
                return View(values);
            }
            ModelState.AddModelError("", "Özellikler yüklenirken bir hata oluştu.");
            return View(new List<ResultFeatureDTO>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateFeatureDTO dTO)
        {
            if (ModelState.IsValid)
            {
                using var content = new MultipartFormDataContent();

                content.Add(new StringContent(dTO.Title ?? string.Empty), "Title");
                content.Add(new StringContent(dTO.SmallTitle ?? string.Empty), "SmallTitle");
                content.Add(new StringContent(dTO.Description ?? string.Empty), "Description");

                if (dTO.ImageFile != null)
                {
                    var fileContent = new StreamContent(dTO.ImageFile.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(dTO.ImageFile.ContentType);
                    content.Add(fileContent, "ImageFile", dTO.ImageFile.FileName);
                }

                var response = await _httpClient.PostAsync("api/Features", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Özellik başarıyla eklendi.";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"API Hatası: {response.ReasonPhrase}. Detay: {errorContent}");
                }
            }
            return View(dTO);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"api/Features/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var apiFeature = JsonConvert.DeserializeObject<GetByIdFeatureDTO>(jsonData);

                if (apiFeature != null)
                {
                    var uiDto = new DTOsLayer.WebUIDTO.FeatureDTO.UpdateFeatureDTO
                    {
                        FeatureId = apiFeature.FeatureId,
                        Title = apiFeature.Title,
                        SmallTitle = apiFeature.SmallTitle,
                        Description = apiFeature.Description,
                        ImageUrl = apiFeature.ImageUrl // API'den gelen tam URL buraya atanıyor.
                    };
                    return View(uiDto);
                }
                else
                {
                    ModelState.AddModelError("", $"ID: {id} ile özellik bulunamadı.");
                    return RedirectToAction("Index");
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                ModelState.AddModelError("", $"ID: {id} ile özellik bulunamadı.");
                return RedirectToAction("Index");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Özellik yüklenirken bir hata oluştu: {errorContent}");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateFeatureDTO dTO, bool removeImage)
        {
            if (ModelState.IsValid)
            {
                using var content = new MultipartFormDataContent();

                content.Add(new StringContent(dTO.FeatureId.ToString()), "FeatureId");
                content.Add(new StringContent(dTO.Title ?? string.Empty), "Title");
                content.Add(new StringContent(dTO.SmallTitle ?? string.Empty), "SmallTitle");
                content.Add(new StringContent(dTO.Description ?? string.Empty), "Description");

                if (removeImage)
                {
                    content.Add(new StringContent("true"), "removeImage");
                }

                if (dTO.ImageFile != null)
                {
                    var fileContent = new StreamContent(dTO.ImageFile.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(dTO.ImageFile.ContentType);
                    content.Add(fileContent, "ImageFile", dTO.ImageFile.FileName);
                }

                var response = await _httpClient.PutAsync("api/Features", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Özellik başarıyla güncellendi.";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"API Hatası: {response.ReasonPhrase}. Detay: {errorContent}");
                }
            }
            return View(dTO);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var responseMessage = await _httpClient.DeleteAsync($"api/Features/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Özellik başarıyla silindi.";
                return RedirectToAction("Index");
            }
            else
            {
                var errorContent = await responseMessage.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Silme işlemi başarısız oldu: {responseMessage.ReasonPhrase}. Detay: {errorContent}");
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/Features/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<DTOsLayer.WebUIDTO.FeatureDTO.GetByIdFeatureDTO>(jsonData);
                return View(value);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                ModelState.AddModelError("", $"ID: {id} ile özellik bulunamadı.");
                return RedirectToAction("Index");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Özellik detayları yüklenirken bir hata oluştu: {errorContent}");
            }
            return View();
        }
    }
}