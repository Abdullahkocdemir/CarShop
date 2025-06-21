using DTOsLayer.WebUIDTO.ServiceDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using System.Net.Http;
using FluentValidation;
using FluentValidation.Results;


namespace CarShop.WebUI.Controllers
{
    public class ServiceController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IValidator<CreateServiceDTO> _createServiceValidator;
        private readonly IValidator<UpdateServiceDTO> _updateServiceValidator;

        public ServiceController(IHttpClientFactory httpClientFactory,
                                 IValidator<CreateServiceDTO> createServiceValidator,
                                 IValidator<UpdateServiceDTO> updateServiceValidator)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
            _createServiceValidator = createServiceValidator;
            _updateServiceValidator = updateServiceValidator;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/Services");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultServiceDTO>>(jsonData);
                return View(values);
            }
            ModelState.AddModelError("", "Hizmetler yüklenirken bir hata oluştu.");
            return View(new List<ResultServiceDTO>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateServiceDTO dto)
        {
            ValidationResult result = await _createServiceValidator.ValidateAsync(dto);

            if (result.IsValid)
            {
                using var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(dto.Title), "Title");
                formData.Add(new StringContent(dto.Description), "Description");

                if (dto.ImageFile != null)
                {
                    var fileContent = new StreamContent(dto.ImageFile.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(dto.ImageFile.ContentType);
                    formData.Add(fileContent, "ImageFile", dto.ImageFile.FileName);
                }

                var response = await _httpClient.PostAsync("api/Services", formData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Hizmet başarıyla eklendi!";
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
            var response = await _httpClient.GetAsync($"api/Services/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var apiServiceDto = JsonConvert.DeserializeObject<GetByIdServiceDTO>(jsonData);

                var updateDto = new UpdateServiceDTO
                {
                    ServiceId = apiServiceDto!.ServiceId,
                    Title = apiServiceDto.Title,
                    Description = apiServiceDto.Description,
                    ExistingImageUrl = apiServiceDto.ImageUrl
                };
                return View(updateDto);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan hizmet bulunamadı.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateServiceDTO dto)
        {
            ValidationResult result = await _updateServiceValidator.ValidateAsync(dto);

            if (result.IsValid)
            {
                using var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(dto.ServiceId.ToString()), "ServiceId");
                formData.Add(new StringContent(dto.Title), "Title");
                formData.Add(new StringContent(dto.Description), "Description");

                if (!string.IsNullOrEmpty(dto.ExistingImageUrl))
                {
                    formData.Add(new StringContent(dto.ExistingImageUrl), "ExistingImageUrl");
                }

                // ImageFile null değilse ekle
                if (dto.ImageFile != null)
                {
                    var fileContent = new StreamContent(dto.ImageFile.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(dto.ImageFile.ContentType);
                    formData.Add(fileContent, "ImageFile", dto.ImageFile.FileName);
                }

                var response = await _httpClient.PutAsync("api/Services", formData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Hizmet başarıyla güncellendi!";
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
            var responseMessage = await _httpClient.DeleteAsync($"api/Services/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Hizmet başarıyla silindi!";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan hizmet silinirken bir hata oluştu.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/Services/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdServiceDTO>(jsonData);
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan hizmet detayları bulunamadı.";
            return RedirectToAction("Index");
        }
    }
}