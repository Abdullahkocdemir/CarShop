using DTOsLayer.WebUIDTO.CallBackDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using FluentValidation;
using FluentValidation.Results;


namespace CarShop.WebUI.Controllers
{
    public class CallBackController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IValidator<CreateCallBackDTO> _createCallBackValidator;
        private readonly IValidator<UpdateCallBackDTO> _updateCallBackValidator;

        public CallBackController(IHttpClientFactory httpClientFactory,
                                  IValidator<CreateCallBackDTO> createCallBackValidator,
                                  IValidator<UpdateCallBackDTO> updateCallBackValidator)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
            _createCallBackValidator = createCallBackValidator;
            _updateCallBackValidator = updateCallBackValidator;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/CallBacks");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultCallBackDTO>>(jsonData);
                return View(values);
            }
            return View(new List<ResultCallBackDTO>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCallBackDTO dto)
        {
            ValidationResult result = await _createCallBackValidator.ValidateAsync(dto);

            if (result.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dto);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/CallBacks", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Geri arama isteği başarıyla eklendi!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "API üzerinde geri arama isteği oluşturulurken bir hata oluştu. Lütfen tekrar deneyin.");
                }
            }
            else
            {
                // Doğrulama hatalarını ModelState'e ekle
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
            var response = await _httpClient.GetAsync($"api/CallBacks/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<UpdateCallBackDTO>(jsonData);
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan geri arama isteği bulunamadı.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateCallBackDTO dto)
        {
            ValidationResult result = await _updateCallBackValidator.ValidateAsync(dto);

            if (result.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dto);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync("api/CallBacks", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Geri arama isteği başarıyla güncellendi!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "API üzerinde geri arama isteği güncellenirken bir hata oluştu. Lütfen tekrar deneyin.");
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
            var responseMessage = await _httpClient.DeleteAsync($"api/CallBacks/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Geri arama isteği başarıyla silindi!";
            }
            else
            {
                TempData["ErrorMessage"] = $"ID'si {id} olan geri arama isteği silinirken bir hata oluştu.";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/CallBacks/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdCallBackDTO>(jsonData);
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan geri arama isteği detayları bulunamadı.";
            return RedirectToAction("Index");
        }
    }
}