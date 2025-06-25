using DTOsLayer.WebUIDTO.ColorDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using FluentValidation;
using FluentValidation.Results;

namespace CarShop.WebUI.Controllers
{
    public class AdminColorController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IValidator<CreateColorDTO> _createColorValidator;
        private readonly IValidator<UpdateColorDTO> _updateColorValidator;

        public AdminColorController(IHttpClientFactory httpClientFactory,
                                    IValidator<CreateColorDTO> createColorValidator,
                                    IValidator<UpdateColorDTO> updateColorValidator)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
            _createColorValidator = createColorValidator;
            _updateColorValidator = updateColorValidator;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/Colors");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultColorDTO>>(jsonData);
                return View(values);
            }
            return View(new List<ResultColorDTO>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateColorDTO dTO)
        {
            ValidationResult result = await _createColorValidator.ValidateAsync(dTO);

            if (result.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dTO);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/Colors", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Renk başarıyla oluşturuldu!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"API üzerinde renk oluşturulurken bir hata oluştu: {errorContent}. Lütfen tekrar deneyin.");
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
            var response = await _httpClient.GetAsync($"api/Colors/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<UpdateColorDTO>(jsonData);
                return View(value);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateColorDTO dTO)
        {
            ValidationResult result = await _updateColorValidator.ValidateAsync(dTO);

            if (result.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dTO);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync("api/Colors", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Renk başarıyla güncellendi!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"API üzerinde renk güncellenirken bir hata oluştu: {errorContent}. Lütfen tekrar deneyin.");
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
            var responseMessage = await _httpClient.DeleteAsync($"api/Colors/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Renk başarıyla silindi!";
            }
            else
            {
                TempData["ErrorMessage"] = "Renk silinirken bir hata oluştu. Lütfen tekrar deneyin.";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/Colors/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdColorDTO>(jsonData);
                return View(value);
            }
            return View();
        }
    }
}