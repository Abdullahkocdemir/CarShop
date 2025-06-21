using DTOsLayer.WebUIDTO.CalltoActionDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using System.Net.Http;
using FluentValidation;
using FluentValidation.Results;

namespace CarShop.WebUI.Controllers
{
    public class CalltoActionController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IValidator<CreateCalltoActionDTO> _createCalltoActionValidator;
        private readonly IValidator<UpdateCalltoActionDTO> _updateCalltoActionValidator;

        public CalltoActionController(IHttpClientFactory httpClientFactory,
                                      IValidator<CreateCalltoActionDTO> createCalltoActionValidator,
                                      IValidator<UpdateCalltoActionDTO> updateCalltoActionValidator)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
            _createCalltoActionValidator = createCalltoActionValidator;
            _updateCalltoActionValidator = updateCalltoActionValidator;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/CalltoActions");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultCalltoActionDTO>>(jsonData);
                return View(values);
            }
            return View(new List<ResultCalltoActionDTO>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCalltoActionDTO dto)
        {
            ValidationResult result = await _createCalltoActionValidator.ValidateAsync(dto);

            if (result.IsValid)
            {

                using var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(dto.Title), "Title");
                formData.Add(new StringContent(dto.SmallTitle), "SmallTitle");

                if (dto.ImageFile != null)
                {
                    var fileContent = new StreamContent(dto.ImageFile.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(dto.ImageFile.ContentType);
                    formData.Add(fileContent, "ImageFile", dto.ImageFile.FileName);
                }

                var response = await _httpClient.PostAsync("api/CalltoActions", formData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Call to Action başarıyla eklendi!";
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
            var response = await _httpClient.GetAsync($"api/CalltoActions/{id}");
            if (response.IsSuccessStatusCode)
            {
                var apiDto = JsonConvert.DeserializeObject<GetByIdCalltoActionDTO>(await response.Content.ReadAsStringAsync());

                var updateDto = new UpdateCalltoActionDTO
                {
                    CalltoActionId = apiDto!.CalltoActionId,
                    Title = apiDto.Title,
                    SmallTitle = apiDto.SmallTitle,
                    ExistingImageUrl = apiDto.ImageUrl
                };
                return View(updateDto);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan Call to Action bulunamadı.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateCalltoActionDTO dto)
        {
            ValidationResult result = await _updateCalltoActionValidator.ValidateAsync(dto);

            if (result.IsValid)
            {
                using var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(dto.CalltoActionId.ToString()), "CalltoActionId");
                formData.Add(new StringContent(dto.Title), "Title");
                formData.Add(new StringContent(dto.SmallTitle), "SmallTitle");

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

                var response = await _httpClient.PutAsync("api/CalltoActions", formData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Call to Action başarıyla güncellendi!";
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
            var responseMessage = await _httpClient.DeleteAsync($"api/CalltoActions/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Call to Action başarıyla silindi!";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan Call to Action silinirken bir hata oluştu.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/CalltoActions/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdCalltoActionDTO>(jsonData);
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan Call to Action detayları bulunamadı.";
            return RedirectToAction("Index");
        }
    }
}