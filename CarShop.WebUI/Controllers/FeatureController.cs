using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using DTOsLayer.WebUIDTO.FeatureDTO;
using DTOsLayer.WebUIDTO.FeatureImageDTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;


namespace CarShop.WebUI.Controllers
{
    public class FeatureController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IValidator<CreateFeatureDTO> _createFeatureValidator;
        private readonly IValidator<UpdateFeatureDTO> _updateFeatureValidator;

        public FeatureController(IHttpClientFactory httpClientFactory,
                                 IValidator<CreateFeatureDTO> createFeatureValidator,
                                 IValidator<UpdateFeatureDTO> updateFeatureValidator)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
            _createFeatureValidator = createFeatureValidator;
            _updateFeatureValidator = updateFeatureValidator;
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
            ValidationResult result = await _createFeatureValidator.ValidateAsync(dTO);

            if (result.IsValid)
            {
                using var content = new MultipartFormDataContent();

                content.Add(new StringContent(dTO.Title ?? string.Empty), "Title");
                content.Add(new StringContent(dTO.SmallTitle ?? string.Empty), "SmallTitle");
                content.Add(new StringContent(dTO.Description ?? string.Empty), "Description");

                if (dTO.ImageFiles != null && dTO.ImageFiles.Any())
                {
                    foreach (var imageFile in dTO.ImageFiles)
                    {
                        var fileContent = new StreamContent(imageFile.OpenReadStream());
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);
                        content.Add(fileContent, "ImageFiles", imageFile.FileName);
                    }
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
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
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
                    var uiDto = new UpdateFeatureDTO
                    {
                        FeatureId = apiFeature.FeatureId,
                        Title = apiFeature.Title,
                        SmallTitle = apiFeature.SmallTitle,
                        Description = apiFeature.Description,
                    };
                    ViewBag.ExistingImages = apiFeature.FeatureImages;
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
        public async Task<IActionResult> Edit(UpdateFeatureDTO dTO, [FromForm] List<int>? ImageIdsToRemove)
        {
            ValidationResult result = await _updateFeatureValidator.ValidateAsync(dTO);

            if (result.IsValid)
            {
                using var content = new MultipartFormDataContent();

                content.Add(new StringContent(dTO.FeatureId.ToString()), "FeatureId");
                content.Add(new StringContent(dTO.Title ?? string.Empty), "Title");
                content.Add(new StringContent(dTO.SmallTitle ?? string.Empty), "SmallTitle");
                content.Add(new StringContent(dTO.Description ?? string.Empty), "Description");

                if (ImageIdsToRemove != null && ImageIdsToRemove.Any())
                {
                    foreach (var imageId in ImageIdsToRemove)
                    {
                        content.Add(new StringContent(imageId.ToString()), "ImageIdsToRemove");
                    }
                }

                if (dTO.NewImageFiles != null && dTO.NewImageFiles.Any())
                {
                    foreach (var newImageFile in dTO.NewImageFiles)
                    {
                        var fileContent = new StreamContent(newImageFile.OpenReadStream());
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(newImageFile.ContentType);
                        content.Add(fileContent, "NewImageFiles", newImageFile.FileName);
                    }
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
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
            }
            var apiResponse = await _httpClient.GetAsync($"api/Features/{dTO.FeatureId}");
            if (apiResponse.IsSuccessStatusCode)
            {
                var jsonData = await apiResponse.Content.ReadAsStringAsync();
                var apiFeature = JsonConvert.DeserializeObject<GetByIdFeatureDTO>(jsonData);
                ViewBag.ExistingImages = apiFeature?.FeatureImages;
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
                var value = JsonConvert.DeserializeObject<GetByIdFeatureDTO>(jsonData);
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