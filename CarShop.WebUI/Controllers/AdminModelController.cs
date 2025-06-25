using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using FluentValidation;
using FluentValidation.Results;
using DTOsLayer.WebUIDTO.ModelsDTO;

namespace CarShop.WebUI.Controllers
{
    public class AdminModelController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IValidator<CreateModelDTO> _createModelValidator;
        private readonly IValidator<UpdateModelDTO> _updateModelValidator;

        public AdminModelController(IHttpClientFactory httpClientFactory,
                                    IValidator<CreateModelDTO> createModelValidator,
                                    IValidator<UpdateModelDTO> updateModelValidator)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
            _createModelValidator = createModelValidator;
            _updateModelValidator = updateModelValidator;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/Models"); 
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultModelDTO>>(jsonData);
                return View(values);
            }
            return View(new List<ResultModelDTO>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateModelDTO dTO)
        {
            ValidationResult result = await _createModelValidator.ValidateAsync(dTO);

            if (result.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dTO);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/Models", content); 

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Model başarıyla oluşturuldu!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"API üzerinde model oluşturulurken bir hata oluştu: {errorContent}. Lütfen tekrar deneyin.");
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
            var response = await _httpClient.GetAsync($"api/Models/{id}"); 
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<UpdateModelDTO>(jsonData);
                return View(value);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateModelDTO dTO)
        {
            ValidationResult result = await _updateModelValidator.ValidateAsync(dTO);

            if (result.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dTO);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync("api/Models", content); 

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Model başarıyla güncellendi!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"API üzerinde model güncellenirken bir hata oluştu: {errorContent}. Lütfen tekrar deneyin.");
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
            var responseMessage = await _httpClient.DeleteAsync($"api/Models/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Model başarıyla silindi!";
            }
            else
            {
                TempData["ErrorMessage"] = "Model silinirken bir hata oluştu. Lütfen tekrar deneyin.";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/Models/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdModelDTO>(jsonData);
                return View(value);
            }
            return View();
        }
    }
}