using DTOsLayer.WebUIDTO.CallBackTitleDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using FluentValidation;
using FluentValidation.Results;


namespace CarShop.WebUI.Controllers
{
    public class CallBackTitleController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IValidator<CreateCallBackTitleDTO> _createCallBackTitleValidator;
        private readonly IValidator<UpdateCallBackTitleDTO> _updateCallBackTitleValidator;

        public CallBackTitleController(IHttpClientFactory httpClientFactory,
                                       IValidator<CreateCallBackTitleDTO> createCallBackTitleValidator,
                                       IValidator<UpdateCallBackTitleDTO> updateCallBackTitleValidator)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
            _createCallBackTitleValidator = createCallBackTitleValidator;
            _updateCallBackTitleValidator = updateCallBackTitleValidator;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/CallBackTitles");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultCallBackTitleDTO>>(jsonData);
                return View(values);
            }
            return View(new List<ResultCallBackTitleDTO>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCallBackTitleDTO dto)
        {
            ValidationResult result = await _createCallBackTitleValidator.ValidateAsync(dto);

            if (result.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dto);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/CallBackTitles", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Geri Arama Başlığı başarıyla eklendi!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "API üzerinde Geri Arama Başlığı oluşturulurken bir hata oluştu. Lütfen tekrar deneyin.");
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
            var response = await _httpClient.GetAsync($"api/CallBackTitles/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<UpdateCallBackTitleDTO>(jsonData);
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan Geri Arama Başlığı bulunamadı.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateCallBackTitleDTO dto)
        {
            ValidationResult result = await _updateCallBackTitleValidator.ValidateAsync(dto);

            if (result.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dto);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync("api/CallBackTitles", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Geri Arama Başlığı başarıyla güncellendi!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "API üzerinde Geri Arama Başlığı güncellenirken bir hata oluştu. Lütfen tekrar deneyin.");
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
            var responseMessage = await _httpClient.DeleteAsync($"api/CallBackTitles/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Geri Arama Başlığı başarıyla silindi!";
            }
            else
            {
                TempData["ErrorMessage"] = $"ID'si {id} olan Geri Arama Başlığı silinirken bir hata oluştu.";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/CallBackTitles/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdCallBackTitleDTO>(jsonData);
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan Geri Arama Başlığı detayları bulunamadı.";
            return RedirectToAction("Index");
        }
    }
}