using DTOsLayer.WebUIDTO.CalltoActionDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using System.Net.Http;

namespace CarShop.WebUI.Controllers
{
    public class CalltoActionController : Controller
    {
        private readonly HttpClient _httpClient;

        public CalltoActionController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
        }
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/CalltoActions");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultCalltoActionDTO>>(jsonData);
                return View(values);
            }
            return View(new List<ResultCalltoActionDTO>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCalltoActionDTO dto)
        {
            if (ModelState.IsValid)
            {
                using var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(dto.Title), "Title");
                formData.Add(new StringContent(dto.SmallTitle), "SmallTitle");

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

                var response = await _httpClient.PostAsync("api/CalltoActions", formData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Call to Action başarıyla eklendi!";
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
            var response = await _httpClient.GetAsync($"api/CalltoActions/{id}");
            if (response.IsSuccessStatusCode)
            {
                var apiDto = JsonConvert.DeserializeObject<GetByIdCalltoActionDTO>(await response.Content.ReadAsStringAsync());

                var updateDto = new UpdateCalltoActionDTO
                {
                    CalltoActionId = apiDto!.CalltoActionId,
                    Title = apiDto.Title,
                    SmallTitle = apiDto.SmallTitle,
                    ExistingImageUrl = apiDto.ImageUrl
                };
                return View(updateDto);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan Call to Action bulunamadı.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateCalltoActionDTO dto)
        {
            if (ModelState.IsValid)
            {
                using var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(dto.CalltoActionId.ToString()), "CalltoActionId");
                formData.Add(new StringContent(dto.Title), "Title");
                formData.Add(new StringContent(dto.SmallTitle), "SmallTitle");

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

                var response = await _httpClient.PutAsync("api/CalltoActions", formData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Call to Action başarıyla güncellendi!";
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
            var responseMessage = await _httpClient.DeleteAsync($"api/CalltoActions/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Call to Action başarıyla silindi!";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan Call to Action silinirken bir hata oluştu.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/CalltoActions/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdCalltoActionDTO>(jsonData);
                return View(value);
            }
            TempData["ErrorMessage"] = $"ID'si {id} olan Call to Action detayları bulunamadı.";
            return RedirectToAction("Index");
        }
    }
}