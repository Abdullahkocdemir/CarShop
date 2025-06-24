using DTOsLayer.WebUIDTO.AboutItemDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using FluentValidation;
using FluentValidation.Results;

namespace CarShop.WebUI.Controllers
{
    public class AdminAboutItemController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IValidator<CreateAboutItemDTO> _createAboutItemValidator;
        private readonly IValidator<UpdateAboutItemDTO> _updateAboutItemValidator;

        public AdminAboutItemController(IHttpClientFactory httpClientFactory,
                                        IValidator<CreateAboutItemDTO> createAboutItemValidator,
                                        IValidator<UpdateAboutItemDTO> updateAboutItemValidator)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
            _createAboutItemValidator = createAboutItemValidator;
            _updateAboutItemValidator = updateAboutItemValidator;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/AboutItems"); 
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultAboutItemDTO>>(jsonData);
                return View(values);
            }
            return View(new List<ResultAboutItemDTO>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAboutItemDTO dTO)
        {
            ValidationResult result = await _createAboutItemValidator.ValidateAsync(dTO);

            if (result.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dTO);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/AboutItems", content); 

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "AboutItem başarıyla oluşturuldu!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"API üzerinde AboutItem oluşturulurken bir hata oluştu: {errorContent}. Lütfen tekrar deneyin.");
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
            var response = await _httpClient.GetAsync($"api/AboutItems/{id}"); 
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<UpdateAboutItemDTO>(jsonData);
                return View(value);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateAboutItemDTO dTO)
        {
            ValidationResult result = await _updateAboutItemValidator.ValidateAsync(dTO);

            if (result.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dTO);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync("api/AboutItems", content); 

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "AboutItem başarıyla güncellendi!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"API üzerinde AboutItem güncellenirken bir hata oluştu: {errorContent}. Lütfen tekrar deneyin.");
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
            var responseMessage = await _httpClient.DeleteAsync($"api/AboutItems/{id}"); 
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "AboutItem başarıyla silindi!";
            }
            else
            {
                TempData["ErrorMessage"] = "AboutItem silinirken bir hata oluştu. Lütfen tekrar deneyin.";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/AboutItems/{id}"); 

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdAboutItemDTO>(jsonData);
                return View(value);
            }
            return View();
        }
    }
}