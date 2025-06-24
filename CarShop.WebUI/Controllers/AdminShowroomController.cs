using DTOsLayer.WebUIDTO.ShowroomDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using FluentValidation;
using FluentValidation.Results;

namespace CarShop.WebUI.Controllers
{
    public class AdminShowroomController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IValidator<CreateShowroomDTO> _createShowroomValidator;
        private readonly IValidator<UpdateShowroomDTO> _updateShowroomValidator;

        public AdminShowroomController(IHttpClientFactory httpClientFactory,
                                       IValidator<CreateShowroomDTO> createShowroomValidator,
                                       IValidator<UpdateShowroomDTO> updateShowroomValidator)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
            _createShowroomValidator = createShowroomValidator;
            _updateShowroomValidator = updateShowroomValidator;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/Showroomss"); 
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultShowroomDTO>>(jsonData);
                return View(values);
            }
            return View(new List<ResultShowroomDTO>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateShowroomDTO dTO)
        {
            ValidationResult result = await _createShowroomValidator.ValidateAsync(dTO);

            if (result.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dTO);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/Showroomss", content); 

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Showroom başarıyla oluşturuldu!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "API üzerinde showroom oluşturulurken bir hata oluştu. Lütfen tekrar deneyin.");
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
            var response = await _httpClient.GetAsync($"api/Showroomss/{id}"); 
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<UpdateShowroomDTO>(jsonData);
                return View(value);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateShowroomDTO dTO)
        {
            ValidationResult result = await _updateShowroomValidator.ValidateAsync(dTO);

            if (result.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dTO);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync("api/Showroomss", content); 

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Showroom başarıyla güncellendi!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "API üzerinde showroom güncellenirken bir hata oluştu. Lütfen tekrar deneyin.");
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
            var responseMessage = await _httpClient.DeleteAsync($"api/Showroomss/{id}"); 
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Showroom başarıyla silindi!";
            }
            else
            {
                TempData["ErrorMessage"] = "Showroom silinirken bir hata oluştu. Lütfen tekrar deneyin.";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/Showroomss/{id}"); 

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdShowroomDTO>(jsonData);
                return View(value);
            }
            return View();
        }
    }
}