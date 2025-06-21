using DTOsLayer.WebUIDTO.NewLatestDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using FluentValidation;
using FluentValidation.Results;


namespace CarShop.WebUI.Controllers
{
    public class NewLatestController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IValidator<CreateNewLatestDTO> _createNewLatestValidator;
        private readonly IValidator<UpdateNewLatestDTO> _updateNewLatestValidator;

        public NewLatestController(IHttpClientFactory httpClientFactory,
                                   IValidator<CreateNewLatestDTO> createNewLatestValidator,
                                   IValidator<UpdateNewLatestDTO> updateNewLatestValidator)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
            _createNewLatestValidator = createNewLatestValidator;
            _updateNewLatestValidator = updateNewLatestValidator;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/NewLatests");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultNewLatestDTO>>(jsonData);
                return View(values);
            }
            ModelState.AddModelError("", "En Son Haberler yüklenirken bir hata oluştu.");
            return View(new List<ResultNewLatestDTO>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateNewLatestDTO dTO)
        {
            ValidationResult result = await _createNewLatestValidator.ValidateAsync(dTO);

            if (result.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dTO);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/NewLatests", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "En Son Haber başarıyla eklendi!";
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
            return View(dTO);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"api/NewLatests/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<UpdateNewLatestDTO>(jsonData);
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan En Son Haber bulunamadı.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateNewLatestDTO dTO)
        {
            ValidationResult result = await _updateNewLatestValidator.ValidateAsync(dTO);

            if (result.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(dTO);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync("api/NewLatests", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "En Son Haber başarıyla güncellendi!";
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
            return View(dTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var responseMessage = await _httpClient.DeleteAsync($"api/NewLatests/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "En Son Haber başarıyla silindi!";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan En Son Haber silinirken bir hata oluştu.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/NewLatests/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdNewLatestDTO>(jsonData);
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan En Son Haber detayları bulunamadı.";
            return RedirectToAction("Index");
        }
    }
}