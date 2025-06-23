using DTOsLayer.WebUIDTO.FeatureDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Results;

namespace CarShop.WebUI.Controllers
{
    public class FeatureController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IValidator<CreateFeatureDTO> _createFeatureValidator;
        private readonly IValidator<UpdateFeatureDTO> _updateFeatureValidator;

        public FeatureController(
            IHttpClientFactory httpClientFactory,
            IValidator<CreateFeatureDTO> createFeatureValidator,
            IValidator<UpdateFeatureDTO> updateFeatureValidator)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
            _createFeatureValidator = createFeatureValidator;
            _updateFeatureValidator = updateFeatureValidator;
        }
        [HttpGet]
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
        public async Task<IActionResult> Create(CreateFeatureDTO dto)
        {
            ValidationResult result = await _createFeatureValidator.ValidateAsync(dto);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(dto);
            }

            var jsonContent = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Features", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Özellik başarıyla eklendi!";
                return RedirectToAction("Index");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"API Hatası: {response.StatusCode} - {errorContent}");
            }

            return View(dto);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"api/Features/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var apiFeatureDto = JsonConvert.DeserializeObject<GetByIdFeatureDTO>(jsonData);

                var updateDto = new UpdateFeatureDTO
                {
                    FeatureId = apiFeatureDto!.FeatureId,
                    Title = apiFeatureDto.Title,
                    SmallDescription = apiFeatureDto.SmallDescription,
                    Description = apiFeatureDto.Description
                };
                return View(updateDto);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan özellik bulunamadı.";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateFeatureDTO dto)
        {
            ValidationResult result = await _updateFeatureValidator.ValidateAsync(dto);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(dto);
            }

            var jsonContent = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("api/Features", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Özellik başarıyla güncellendi!";
                return RedirectToAction("Index");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"API Hatası: {response.StatusCode} - {errorContent}");
            }

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var responseMessage = await _httpClient.DeleteAsync($"api/Features/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Özellik başarıyla silindi!";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan özellik silinirken bir hata oluştu.";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/Features/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdFeatureDTO>(jsonData);
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan özellik detayları bulunamadı.";
            return RedirectToAction("Index");
        }
    }
}