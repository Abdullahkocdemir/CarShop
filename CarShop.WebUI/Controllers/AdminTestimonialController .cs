using DTOsLayer.WebUIDTO.TestimonialDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using FluentValidation;
using FluentValidation.Results;

namespace CarShop.WebUI.Controllers
{
    public class AdminTestimonialController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IValidator<CreateTestimonialDTO> _createTestimonialValidator;
        private readonly IValidator<UpdateTestimonialDTO> _updateTestimonialValidator;

        public AdminTestimonialController(IHttpClientFactory httpClientFactory,
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
            return View(new List<ResultTestimonialDTO>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTestimonialDTO dTO)
        {
            ValidationResult result = await _createTestimonialValidator.ValidateAsync(dTO);

            if (result.IsValid)
            {
                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(dTO.NameSurname), "NameSurname");
                formData.Add(new StringContent(dTO.Duty), "Duty");
                formData.Add(new StringContent(dTO.Description), "Description");

                if (dTO.ImageFile != null)
                {
                    var fileContent = new StreamContent(dTO.ImageFile.OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(dTO.ImageFile.ContentType);
                    formData.Add(fileContent, "ImageFile", dTO.ImageFile.FileName);
                }

                var response = await _httpClient.PostAsync("api/Testimonials", formData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Testimonial başarıyla oluşturuldu!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"API üzerinde Testimonial oluşturulurken bir hata oluştu: {errorContent}. Lütfen tekrar deneyin.");
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
            var response = await _httpClient.GetAsync($"api/Testimonials/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<UpdateTestimonialDTO>(jsonData);
                var getByIdDto = JsonConvert.DeserializeObject<GetByIdTestimonialDTO>(jsonData);
                if (getByIdDto != null)
                {
                    value!.ExistingImageUrl = getByIdDto.ImageUrl;
                }
                return View(value);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateTestimonialDTO dTO)
        {
            ValidationResult result = await _updateTestimonialValidator.ValidateAsync(dTO);

            if (result.IsValid)
            {
                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(dTO.TestimonialId.ToString()), "TestimonialId");
                formData.Add(new StringContent(dTO.NameSurname), "NameSurname");
                formData.Add(new StringContent(dTO.Duty), "Duty");
                formData.Add(new StringContent(dTO.Description), "Description");

                if (dTO.ImageFile != null)
                {
                    var fileContent = new StreamContent(dTO.ImageFile.OpenReadStream());
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(dTO.ImageFile.ContentType);
                    formData.Add(fileContent, "ImageFile", dTO.ImageFile.FileName);
                }
                else if (!string.IsNullOrEmpty(dTO.ExistingImageUrl))
                {
                    formData.Add(new StringContent(dTO.ExistingImageUrl), "ExistingImageUrl");
                }

                var response = await _httpClient.PutAsync("api/Testimonials", formData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Testimonial başarıyla güncellendi!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"API üzerinde Testimonial güncellenirken bir hata oluştu: {errorContent}. Lütfen tekrar deneyin.");
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
        public async Task<IActionResult> Delete(int id)
        {
            var responseMessage = await _httpClient.DeleteAsync($"api/Testimonials/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Testimonial başarıyla silindi!";
            }
            else
            {
                TempData["ErrorMessage"] = "Testimonial silinirken bir hata oluştu. Lütfen tekrar deneyin.";
            }
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
            return View();
        }
    }
}