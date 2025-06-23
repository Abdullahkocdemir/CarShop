using DTOsLayer.WebUIDTO.WhyUseDTO; 
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using FluentValidation;
using FluentValidation.Results;
using DTOsLayer.WebUIDTO.WhyUseItemDTO;

namespace CarShop.WebUI.Controllers
{
    public class WhyUseController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IValidator<CreateWhyUseDTO> _createWhyUseValidator;
        private readonly IValidator<UpdateWhyUseDTO> _updateWhyUseValidator;

        public WhyUseController(IHttpClientFactory httpClientFactory,
                                 IValidator<CreateWhyUseDTO> createWhyUseValidator,
                                 IValidator<UpdateWhyUseDTO> updateWhyUseValidator)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
            _createWhyUseValidator = createWhyUseValidator;
            _updateWhyUseValidator = updateWhyUseValidator;
        }

        // Listeleme Sayfası
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/WhyUses");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultWhyUseDTO>>(jsonData);
                return View(values);
            }
            return View(new List<ResultWhyUseDTO>());
        }

        // Yeni Ekle (GET)
        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateWhyUseDTO();
            model.Items.Add(new CreateWhyUseItemDTO());
            return View(model);
        }

        // Yeni Ekle (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateWhyUseDTO dTO)
        {
            dTO.Items = dTO.Items?.Where(item => !string.IsNullOrWhiteSpace(item.Content)).ToList() ?? new List<CreateWhyUseItemDTO>();

            ValidationResult result = await _createWhyUseValidator.ValidateAsync(dTO);

            if (result.IsValid)
            {
                var apiDto = new CreateWhyUseDTO
                {
                    MainTitle = dTO.MainTitle,
                    MainDescription = dTO.MainDescription,
                    VideoUrl = dTO.VideoUrl,
                    Items = dTO.Items.Select(item => new CreateWhyUseItemDTO { Content = item.Content }).ToList()
                };

                var jsonData = JsonConvert.SerializeObject(apiDto);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/WhyUses", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "WhyUse içeriği başarıyla oluşturuldu!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "API üzerinde WhyUse içeriği oluşturulurken bir hata oluştu. Lütfen tekrar deneyin.");
                    var errorContent = await response.Content.ReadAsStringAsync();
                }
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
            }
            if (!dTO.Items.Any())
            {
                dTO.Items.Add(new CreateWhyUseItemDTO());
            }
            return View(dTO);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"api/WhyUses/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var apiDto = JsonConvert.DeserializeObject<GetByIdWhyUseDTO>(jsonData);
                var uiDto = new UpdateWhyUseDTO
                {
                    WhyUseId = apiDto!.WhyUseId,
                    MainTitle = apiDto.MainTitle,
                    MainDescription = apiDto.MainDescription,
                    VideoUrl = apiDto.VideoUrl,
                    Items = apiDto.Items.Select(item => new UpdateWhyUseItemDTO { Id = item.Id, Content = item.Content, IsDeleted = false }).ToList()
                };

                if (!uiDto.Items.Any())
                {
                    uiDto.Items.Add(new UpdateWhyUseItemDTO());
                }

                return View(uiDto);
            }
            return NotFound(); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateWhyUseDTO dTO)
        {
            dTO.Items = dTO.Items?.Where(item => !string.IsNullOrWhiteSpace(item.Content) || item.IsDeleted).ToList() ?? new List<UpdateWhyUseItemDTO>();

            ValidationResult result = await _updateWhyUseValidator.ValidateAsync(dTO);

            if (result.IsValid)
            {
                var apiDto = new UpdateWhyUseDTO
                {
                    WhyUseId = dTO.WhyUseId,
                    MainTitle = dTO.MainTitle,
                    MainDescription = dTO.MainDescription,
                    VideoUrl = dTO.VideoUrl,
                    Items = dTO.Items.Select(item => new UpdateWhyUseItemDTO
                    {
                        Id = item.Id,
                        Content = item.Content
                    }).Where(item => !dTO.Items.First(uiItem => uiItem.Id == item.Id || uiItem.Content == item.Content && item.Id == 0).IsDeleted).ToList()
                };

                var jsonData = JsonConvert.SerializeObject(apiDto);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync("api/WhyUses", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "WhyUse içeriği başarıyla güncellendi!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "API üzerinde WhyUse içeriği güncellenirken bir hata oluştu. Lütfen tekrar deneyin.");
                    var errorContent = await response.Content.ReadAsStringAsync();
                }
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
            }
            if (!dTO.Items.Any())
            {
                dTO.Items.Add(new UpdateWhyUseItemDTO());
            }
            return View(dTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var responseMessage = await _httpClient.DeleteAsync($"api/WhyUses/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "WhyUse içeriği başarıyla silindi!";
            }
            else
            {
                TempData["ErrorMessage"] = "WhyUse içeriği silinirken bir hata oluştu. Lütfen tekrar deneyin.";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/WhyUses/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdWhyUseDTO>(jsonData);
                return View(value);
            }
            return NotFound();
        }
    }
}