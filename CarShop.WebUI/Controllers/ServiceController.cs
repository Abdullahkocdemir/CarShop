using DTOsLayer.WebUIDTO.ServiceDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace CarShop.WebUI.Controllers
{
    public class ServiceController : Controller
    {
        private readonly HttpClient _httpClient;

        public ServiceController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/Services");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var values = JsonConvert.DeserializeObject<List<ResultServiceDTO>>(jsonData);
                return View(values);
            }
            return View(); // Or handle error
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateServiceDTO dto)
        {
            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(dto.Title), "Title");
            formData.Add(new StringContent(dto.Description), "Description");

            if (dto.ImageFile != null)
            {
                var fileContent = new StreamContent(dto.ImageFile.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(dto.ImageFile.ContentType);
                formData.Add(fileContent, "ImageFile", dto.ImageFile.FileName);
            }

            var response = await _httpClient.PostAsync("api/Services", formData);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"API Hata: {response.StatusCode} - {errorContent}");
            }
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"api/Services/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                // 1. Deserialize to GetByIdServiceDTO, as this matches the API's return structure for a single service
                var apiServiceDto = JsonConvert.DeserializeObject<GetByIdServiceDTO>(jsonData);

                // 2. Map data from apiServiceDto to your Web UI's UpdateServiceDTO
                var updateDto = new UpdateServiceDTO
                {
                    ServiceId = apiServiceDto!.ServiceId,
                    Title = apiServiceDto.Title,
                    Description = apiServiceDto.Description,
                    ExistingImageUrl = apiServiceDto.ImageUrl // Now correctly assigning from API's ImageUrl
                };
                return View(updateDto);
            }
            return View(); // Or handle error
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateServiceDTO dto)
        {
            using var formData = new MultipartFormDataContent();
            formData.Add(new StringContent(dto.ServiceId.ToString()), "ServiceId");
            formData.Add(new StringContent(dto.Title), "Title");
            formData.Add(new StringContent(dto.Description), "Description");
            if (!string.IsNullOrEmpty(dto.ExistingImageUrl))
            {
                formData.Add(new StringContent(dto.ExistingImageUrl), "ExistingImageUrl");
            }

            if (dto.ImageFile != null)
            {
                var fileContent = new StreamContent(dto.ImageFile.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(dto.ImageFile.ContentType);
                formData.Add(fileContent, "ImageFile", dto.ImageFile.FileName);
            }

            var response = await _httpClient.PutAsync("api/Services", formData);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"API Hata: {response.StatusCode} - {errorContent}");
            }
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var responseMessage = await _httpClient.DeleteAsync($"api/Services/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            // Handle error, maybe show an error message
            return View(); // Or redirect with an error message
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/Services/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var value = JsonConvert.DeserializeObject<GetByIdServiceDTO>(jsonData);
                return View(value);
            }
            return View(); // Or handle error
        }
    }
}