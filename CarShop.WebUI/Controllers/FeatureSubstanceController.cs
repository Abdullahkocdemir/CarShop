using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using FluentValidation;
using FluentValidation.Results;
using DTOsLayer.WebUIDTO.FeatureSubstancesDTO; // FluentValidation kullanıyorsanız

namespace CarShop.WebUI.Controllers
{
    public class FeatureSubstanceController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IValidator<CreateFeatureSubstancesDTO> _createFeatureSubstanceValidator;
        private readonly IValidator<UpdateFeatureSubstancesDTO> _updateFeatureSubstanceValidator;

        public FeatureSubstanceController(
            IHttpClientFactory httpClientFactory,
            IValidator<CreateFeatureSubstancesDTO> createFeatureSubstanceValidator,
            IValidator<UpdateFeatureSubstancesDTO> updateFeatureSubstanceValidator)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient"); 
            _createFeatureSubstanceValidator = createFeatureSubstanceValidator;
            _updateFeatureSubstanceValidator = updateFeatureSubstanceValidator;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/FeatureSubstances");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultFeatureSubstancesDTO>>(jsonData);
                return View(values);
            }
            ModelState.AddModelError("", "Özellik maddeleri yüklenirken bir hata oluştu.");
            return View(new List<ResultFeatureSubstancesDTO>());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateFeatureSubstancesDTO dto)
        {
            ValidationResult result = await _createFeatureSubstanceValidator.ValidateAsync(dto);

            if (result.IsValid)
            {
                using var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(dto.Subject), "Subject");

                if (dto.ImageFile != null)
                {
                    var fileContent = new StreamContent(dto.ImageFile.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(dto.ImageFile.ContentType);
                    formData.Add(fileContent, "ImageFile", dto.ImageFile.FileName);
                }
                else
                {
                    ModelState.AddModelError("ImageFile", "Resim dosyası gereklidir.");
                    return View(dto);
                }

                var response = await _httpClient.PostAsync("api/FeatureSubstances", formData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Özellik maddesi başarıyla eklendi!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"API Hatası: {response.StatusCode} - {errorContent}");
                }
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
            }
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"api/FeatureSubstances/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var apiFeatureSubstanceDto = JsonConvert.DeserializeObject<GetByIdFeatureSubstancesDTO>(jsonData);

                var updateDto = new UpdateFeatureSubstancesDTO
                {
                    FeatureSubstanceId = apiFeatureSubstanceDto!.FeatureSubstanceId,
                    Subject = apiFeatureSubstanceDto.Subject,
                    ExistingImageUrl = apiFeatureSubstanceDto.ImageUrl 
                };
                return View(updateDto);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan özellik maddesi bulunamadı.";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateFeatureSubstancesDTO dto)
        {
            ValidationResult result = await _updateFeatureSubstanceValidator.ValidateAsync(dto);

            if (result.IsValid)
            {
                using var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(dto.FeatureSubstanceId.ToString()), "FeatureSubstanceId");
                formData.Add(new StringContent(dto.Subject), "Subject");

                if (!string.IsNullOrEmpty(dto.ExistingImageUrl) && dto.ImageFile == null)
                {
                    formData.Add(new StringContent(dto.ExistingImageUrl), "ExistingImageUrl");
                }

                if (dto.ImageFile != null)
                {
                    var fileContent = new StreamContent(dto.ImageFile.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(dto.ImageFile.ContentType);
                    formData.Add(fileContent, "ImageFile", dto.ImageFile.FileName);
                }

                var response = await _httpClient.PutAsync("api/FeatureSubstances", formData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Özellik maddesi başarıyla güncellendi!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"API Hatası: {response.StatusCode} - {errorContent}");
                }
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
            }
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var responseMessage = await _httpClient.DeleteAsync($"api/FeatureSubstances/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Özellik maddesi başarıyla silindi!";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan özellik maddesi silinirken bir hata oluştu.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/FeatureSubstances/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdFeatureSubstancesDTO>(jsonData);
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan özellik maddesi detayları bulunamadı.";
            return RedirectToAction("Index");
        }
    }
}