using DTOsLayer.WebUIDTO.PartnerDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using System.Net.Http;
using FluentValidation;
using FluentValidation.Results;


namespace CarShop.WebUI.Controllers
{
    public class PartnerController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IValidator<CreatePartnerDTO> _createPartnerValidator;
        private readonly IValidator<UpdatePartnerDTO> _updatePartnerValidator;

        public PartnerController(IHttpClientFactory httpClientFactory,
                                 IValidator<CreatePartnerDTO> createPartnerValidator,
                                 IValidator<UpdatePartnerDTO> updatePartnerValidator)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
            _createPartnerValidator = createPartnerValidator;
            _updatePartnerValidator = updatePartnerValidator;
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
            ModelState.AddModelError("", "Ortaklar yüklenirken bir hata oluştu.");
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
            ValidationResult result = await _createPartnerValidator.ValidateAsync(dto);

            if (result.IsValid)
            {
                using var formData = new MultipartFormDataContent();

                if (dto.ImageFile != null)
                {
                    var fileContent = new StreamContent(dto.ImageFile.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(dto.ImageFile.ContentType);
                    formData.Add(fileContent, "ImageFile", dto.ImageFile.FileName);
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
            ValidationResult result = await _updatePartnerValidator.ValidateAsync(dto);

            if (result.IsValid)
            {
                using var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(dto.PartnerId.ToString()), "PartnerId");

                if (!string.IsNullOrEmpty(dto.ExistingImageUrl))
                {
                    formData.Add(new StringContent(dto.ExistingImageUrl), "ExistingImageUrl");
                }

                if (dto.ImageFile != null)
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