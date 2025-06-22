using DTOsLayer.WebUIDTO.TestimonialDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using FluentValidation;
using FluentValidation.Results;


namespace CarShop.WebUI.Controllers
{
    public class TestimonialController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IValidator<CreateTestimonialDTO> _createTestimonialValidator;
        private readonly IValidator<UpdateTestimonialDTO> _updateTestimonialValidator;

        public TestimonialController(IHttpClientFactory httpClientFactory,
                                     IValidator<CreateTestimonialDTO> createTestimonialValidator,
                                     IValidator<UpdateTestimonialDTO> updateTestimonialValidator)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
            _createTestimonialValidator = createTestimonialValidator;
            _updateTestimonialValidator = updateTestimonialValidator;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/Testimonials");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultTestimonialDTO>>(jsonData);
                return View(values);
            }
            ModelState.AddModelError("", "Referanslar yüklenirken bir hata oluştu.");
            return View(new List<ResultTestimonialDTO>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTestimonialDTO dto)
        {
            ValidationResult result = await _createTestimonialValidator.ValidateAsync(dto);

            if (result.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dto);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/Testimonials", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Referans başarıyla eklendi!";
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
            var response = await _httpClient.GetAsync($"api/Testimonials/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<UpdateTestimonialDTO>(jsonData);
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan referans bulunamadı.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateTestimonialDTO dto)
        {
            ValidationResult result = await _updateTestimonialValidator.ValidateAsync(dto);

            if (result.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dto);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync("api/Testimonials", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Referans başarıyla güncellendi!";
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
            var responseMessage = await _httpClient.DeleteAsync($"api/Testimonials/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Referans başarıyla silindi!";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan referans silinirken bir hata oluştu.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/Testimonials/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdTestimonialDTO>(jsonData);
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan referans detayları bulunamadı.";
            return RedirectToAction("Index");
        }
    }
}