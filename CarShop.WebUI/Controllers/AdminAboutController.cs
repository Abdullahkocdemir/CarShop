using DTOsLayer.WebUIDTO.AboutDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using FluentValidation;
using FluentValidation.Results;

namespace CarShop.WebUI.Controllers
{
    public class AdminAboutController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IValidator<CreateAboutDTO> _createAboutValidator;
        private readonly IValidator<UpdateAboutDTO> _updateAboutValidator;

        public AdminAboutController(IHttpClientFactory httpClientFactory,
                                    IValidator<CreateAboutDTO> createAboutValidator,
                                    IValidator<UpdateAboutDTO> updateAboutValidator)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
            _createAboutValidator = createAboutValidator;
            _updateAboutValidator = updateAboutValidator;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/Abouts"); 
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultAboutDTO>>(jsonData);
                return View(values);
            }
            return View(new List<ResultAboutDTO>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAboutDTO dTO)
        {
            ValidationResult result = await _createAboutValidator.ValidateAsync(dTO);

            if (result.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dTO);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/Abouts", content); 

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "About girişi başarıyla oluşturuldu!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"API üzerinde About girişi oluşturulurken bir hata oluştu: {errorContent}. Lütfen tekrar deneyin.");
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
            var response = await _httpClient.GetAsync($"api/Abouts/{id}"); 
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<UpdateAboutDTO>(jsonData);
                return View(value);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateAboutDTO dTO)
        {
            ValidationResult result = await _updateAboutValidator.ValidateAsync(dTO);

            if (result.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dTO);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync("api/Abouts", content); 

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "About girişi başarıyla güncellendi!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"API üzerinde About girişi güncellenirken bir hata oluştu: {errorContent}. Lütfen tekrar deneyin.");
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
            var responseMessage = await _httpClient.DeleteAsync($"api/Abouts/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "About girişi başarıyla silindi!";
            }
            else
            {
                TempData["ErrorMessage"] = "About girişi silinirken bir hata oluştu. Lütfen tekrar deneyin.";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/Abouts/{id}"); 

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdAboutDTO>(jsonData);
                return View(value);
            }
            return View();
        }
    }
}