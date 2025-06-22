using DTOsLayer.WebUIDTO.BrandDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using FluentValidation;
using FluentValidation.Results;


namespace CarShop.WebUI.Controllers
{
    public class BrandController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IValidator<CreateBrandDTO> _createBrandValidator;
        private readonly IValidator<UpdateBrandDTO> _updateBrandValidator;

        public BrandController(IHttpClientFactory httpClientFactory,
                               IValidator<CreateBrandDTO> createBrandValidator,
                               IValidator<UpdateBrandDTO> updateBrandValidator)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
            _createBrandValidator = createBrandValidator;
            _updateBrandValidator = updateBrandValidator;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/Brands");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultBrandDTO>>(jsonData);
                return View(values);
            }
            return View(new List<ResultBrandDTO>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBrandDTO dTO)
        {
            ValidationResult result = await _createBrandValidator.ValidateAsync(dTO);

            if (result.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dTO);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/Brands", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Marka başarıyla oluşturuldu!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "API üzerinde marka oluşturulurken bir hata oluştu. Lütfen tekrar deneyin.");
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
            var response = await _httpClient.GetAsync($"api/Brands/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<UpdateBrandDTO>(jsonData);
                return View(value);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateBrandDTO dTO)
        {
            ValidationResult result = await _updateBrandValidator.ValidateAsync(dTO);

            if (result.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dTO);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync("api/Brands", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Marka başarıyla güncellendi!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "API üzerinde marka güncellenirken bir hata oluştu. Lütfen tekrar deneyin.");
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
            var responseMessage = await _httpClient.DeleteAsync($"api/Brands/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Marka başarıyla silindi!";
            }
            else
            {
                TempData["ErrorMessage"] = "Marka silinirken bir hata oluştu. Lütfen tekrar deneyin.";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/Brands/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdBrandDTO>(jsonData);
                return View(value);
            }
            return View();
        }
    }
}