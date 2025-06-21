using DTOsLayer.WebUIDTO.StaffDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using System.Net.Http;

namespace CarShop.WebUI.Controllers
{
    public class StaffController : Controller
    {
        private readonly HttpClient _httpClient;

        public StaffController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
        }


        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/Staffs");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultStaffDTO>>(jsonData);
                return View(values);
            }
            return View(new List<ResultStaffDTO>());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateStaffDTO dto)
        {
            if (ModelState.IsValid)
            {
                using var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(dto.Name), "Name");
                formData.Add(new StringContent(dto.Duty), "Duty");

                if (dto.ImageFile != null && dto.ImageFile.Length > 0)
                {
                    var fileContent = new StreamContent(dto.ImageFile.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(dto.ImageFile.ContentType);
                    formData.Add(fileContent, "ImageFile", dto.ImageFile.FileName);
                }
                else
                {
                    ModelState.AddModelError("ImageFile", "Lütfen bir resim dosyası yükleyin.");
                    return View(dto);
                }

                var response = await _httpClient.PostAsync("api/Staffs", formData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Personel başarıyla eklendi!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"API Hatası: {response.StatusCode} - {errorContent}");
                }
            }
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"api/Staffs/{id}");
            if (response.IsSuccessStatusCode)
            {
                var apiDto = JsonConvert.DeserializeObject<GetByIdStaffDTO>(await response.Content.ReadAsStringAsync());

                var updateDto = new UpdateStaffDTO
                {
                    StaffId = apiDto!.StaffId,
                    Name = apiDto.Name,
                    Duty = apiDto.Duty,
                    ExistingImageUrl = apiDto.ImageUrl
                };
                return View(updateDto);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan personel bulunamadı.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateStaffDTO dto)
        {
            if (ModelState.IsValid)
            {
                using var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(dto.StaffId.ToString()), "StaffId");
                formData.Add(new StringContent(dto.Name), "Name");
                formData.Add(new StringContent(dto.Duty), "Duty");

                if (!string.IsNullOrEmpty(dto.ExistingImageUrl))
                {
                    formData.Add(new StringContent(dto.ExistingImageUrl), "ExistingImageUrl");
                }

                if (dto.ImageFile != null && dto.ImageFile.Length > 0)
                {
                    var fileContent = new StreamContent(dto.ImageFile.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(dto.ImageFile.ContentType);
                    formData.Add(fileContent, "ImageFile", dto.ImageFile.FileName);
                }

                var response = await _httpClient.PutAsync("api/Staffs", formData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Personel başarıyla güncellendi!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"API Hatası: {response.StatusCode} - {errorContent}");
                }
            }
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var responseMessage = await _httpClient.DeleteAsync($"api/Staffs/{id}"); 
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Personel başarıyla silindi!";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan personel silinirken bir hata oluştu.";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/Staffs/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdStaffDTO>(jsonData);
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan personel detayları bulunamadı.";
            return RedirectToAction("Index");
        }

    }
}