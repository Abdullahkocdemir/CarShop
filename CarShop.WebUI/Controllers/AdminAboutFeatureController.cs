using DTOsLayer.WebUIDTO.AboutFeatureDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using FluentValidation;
using FluentValidation.Results;

namespace CarShop.WebUI.Controllers
{
    public class AdminAboutFeatureController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IValidator<CreateAboutFeatureDTO> _createAboutFeatureValidator;
        private readonly IValidator<UpdateAboutFeatureDTO> _updateAboutFeatureValidator;

        public AdminAboutFeatureController(IHttpClientFactory httpClientFactory,
                                           IValidator<CreateAboutFeatureDTO> createAboutFeatureValidator,
                                           IValidator<UpdateAboutFeatureDTO> updateAboutFeatureValidator)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
            _createAboutFeatureValidator = createAboutFeatureValidator;
            _updateAboutFeatureValidator = updateAboutFeatureValidator;
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
        public async Task<IActionResult> Create(CreateAboutFeatureDTO dTO)
        {
            ValidationResult result = await _createAboutFeatureValidator.ValidateAsync(dTO);

            if (result.IsValid)
            {
                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(dTO.Title), "Title");
                formData.Add(new StringContent(dTO.Description), "Description");

                if (dTO.ImageFile != null)
                {
                    var fileContent = new StreamContent(dTO.ImageFile.OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(dTO.ImageFile.ContentType);
                    formData.Add(fileContent, "ImageFile", dTO.ImageFile.FileName);
                }

                var response = await _httpClient.PostAsync("api/AboutFeatures", formData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "AboutFeature başarıyla oluşturuldu!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"API üzerinde AboutFeature oluşturulurken bir hata oluştu: {errorContent}. Lütfen tekrar deneyin.");
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
            var response = await _httpClient.GetAsync($"api/AboutFeatures/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<UpdateAboutFeatureDTO>(jsonData);
                var getByIdDto = JsonConvert.DeserializeObject<GetByIdAboutFeatureDTO>(jsonData);
                if (getByIdDto != null)
                {
                    value!.ExistingImageUrl = getByIdDto.ImageUrl;
                }
                return View(value);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateAboutFeatureDTO dTO)
        {
            ValidationResult result = await _updateAboutFeatureValidator.ValidateAsync(dTO);

            if (result.IsValid)
            {
                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(dTO.AboutFeatureId.ToString()), "AboutFeatureId");
                formData.Add(new StringContent(dTO.Title), "Title");
                formData.Add(new StringContent(dTO.Description), "Description");

                if (dTO.ImageFile != null)
                {
                    var fileContent = new StreamContent(dTO.ImageFile.OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(dTO.ImageFile.ContentType);
                    formData.Add(fileContent, "ImageFile", dTO.ImageFile.FileName);
                }
                else if (!string.IsNullOrEmpty(dTO.ExistingImageUrl))
                {
                    formData.Add(new StringContent(dTO.ExistingImageUrl), "ExistingImageUrl");
                }


                var response = await _httpClient.PutAsync("api/AboutFeatures", formData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "AboutFeature başarıyla güncellendi!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"API üzerinde AboutFeature güncellenirken bir hata oluştu: {errorContent}. Lütfen tekrar deneyin.");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var responseMessage = await _httpClient.DeleteAsync($"api/AboutFeatures/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "AboutFeature başarıyla silindi!";
            }
            else
            {
                TempData["ErrorMessage"] = "AboutFeature silinirken bir hata oluştu. Lütfen tekrar deneyin.";
            }
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
            return View();
        }
    }
}