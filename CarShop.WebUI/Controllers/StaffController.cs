using DTOsLayer.WebUIDTO.StaffDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using System.Net.Http;
using FluentValidation;
using FluentValidation.Results;


namespace CarShop.WebUI.Controllers
{
    public class StaffController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IValidator<CreateStaffDTO> _createStaffValidator;
        private readonly IValidator<UpdateStaffDTO> _updateStaffValidator;

        public StaffController(IHttpClientFactory httpClientFactory,
                               IValidator<CreateStaffDTO> createStaffValidator,
                               IValidator<UpdateStaffDTO> updateStaffValidator)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
            _createStaffValidator = createStaffValidator;
            _updateStaffValidator = updateStaffValidator;
        }


        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/Staffs");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultStaffDTO>>(jsonData);
                return View(values);
            }
            ModelState.AddModelError("", "Personel listesi yüklenirken bir hata oluştu.");
            return View(new List<ResultStaffDTO>());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateStaffDTO dto)
        {
            ValidationResult result = await _createStaffValidator.ValidateAsync(dto);

            if (result.IsValid)
            {
                using var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(dto.Name), "Name");
                formData.Add(new StringContent(dto.Duty), "Duty");

                if (dto.ImageFile != null)
                {
                    var fileContent = new StreamContent(dto.ImageFile.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(dto.ImageFile.ContentType);
                    formData.Add(fileContent, "ImageFile", dto.ImageFile.FileName);
                }

                var response = await _httpClient.PostAsync("api/Staffs", formData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Personel başarıyla eklendi!";
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
            var response = await _httpClient.GetAsync($"api/Staffs/{id}");
            if (response.IsSuccessStatusCode)
            {
                var apiDto = JsonConvert.DeserializeObject<GetByIdStaffDTO>(await response.Content.ReadAsStringAsync());

                var updateDto = new UpdateStaffDTO
                {
                    StaffId = apiDto!.StaffId,
                    Name = apiDto.Name,
                    Duty = apiDto.Duty,
                    ExistingImageUrl = apiDto.ImageUrl
                };
                return View(updateDto);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan personel bulunamadı.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateStaffDTO dto)
        {
            ValidationResult result = await _updateStaffValidator.ValidateAsync(dto);

            if (result.IsValid)
            {
                using var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(dto.StaffId.ToString()), "StaffId");
                formData.Add(new StringContent(dto.Name), "Name");
                formData.Add(new StringContent(dto.Duty), "Duty");

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

                var response = await _httpClient.PutAsync("api/Staffs", formData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Personel başarıyla güncellendi!";
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
            var responseMessage = await _httpClient.DeleteAsync($"api/Staffs/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Personel başarıyla silindi!";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan personel silinirken bir hata oluştu.";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/Staffs/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdStaffDTO>(jsonData);
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan personel detayları bulunamadı.";
            return RedirectToAction("Index");
        }

    }
}