using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using AutoMapper;
using DTOsLayer.WebUIDTO.BannerDTO; 

namespace CarShop.WebUI.Controllers
{
    public class BannerController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper; 

        public BannerController(IHttpClientFactory httpClientFactory, IMapper mapper)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
            _mapper = mapper; 
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/Banners");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var webApiBanners = JsonConvert.DeserializeObject<List<ResultBannerDTO>>(jsonData);
                var uiBanners = _mapper.Map<List<ResultBannerDTO>>(webApiBanners);
                return View(uiBanners);
            }
            return View(new List<ResultBannerDTO>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBannerDTO uiDto)
        {
            var apiDto = _mapper.Map<DTOsLayer.WebApiDTO.BannerDTO.CreateBannerDTO>(uiDto);

            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent(apiDto.SmallTitle ?? string.Empty), "SmallTitle");
                formData.Add(new StringContent(apiDto.SubTitle ?? string.Empty), "SubTitle");
                formData.Add(new StringContent(apiDto.CarModel ?? string.Empty), "CarModel");
                formData.Add(new StringContent(apiDto.Month ?? string.Empty), "Month");
                formData.Add(new StringContent(apiDto.Price ?? string.Empty), "Price");

                if (uiDto.CarImage != null)
                {
                    var fileContent = new StreamContent(uiDto.CarImage.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(uiDto.CarImage.ContentType);
                    formData.Add(fileContent, "CarImage", uiDto.CarImage.FileName);
                }

                if (uiDto.LogoImage != null)
                {
                    var fileContent = new StreamContent(uiDto.LogoImage.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(uiDto.LogoImage.ContentType);
                    formData.Add(fileContent, "LogoImage", uiDto.LogoImage.FileName);
                }

                var response = await _httpClient.PostAsync("api/Banners", formData);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"API Hatası: {response.StatusCode} - {errorContent}");
                }
            }
            return View(uiDto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"api/Banners/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var apiBanner = JsonConvert.DeserializeObject<DTOsLayer.WebApiDTO.BannerDTO.ResultBannerDTO>(jsonData); 
                var updateDto = _mapper.Map<UpdateBannerDTO>(apiBanner);

                return View(updateDto);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateBannerDTO uiDto)
        {
            var apiDto = _mapper.Map<DTOsLayer.WebApiDTO.BannerDTO.UpdateBannerDTO>(uiDto);

            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent(uiDto.BannerId.ToString()), "BannerId");
                formData.Add(new StringContent(uiDto.SmallTitle ?? string.Empty), "SmallTitle");
                formData.Add(new StringContent(uiDto.SubTitle ?? string.Empty), "SubTitle");
                formData.Add(new StringContent(uiDto.CarModel ?? string.Empty), "CarModel");
                formData.Add(new StringContent(uiDto.Month ?? string.Empty), "Month");
                formData.Add(new StringContent(uiDto.Price ?? string.Empty), "Price");

                if (uiDto.CarImage != null)
                {
                    var fileContent = new StreamContent(uiDto.CarImage.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(uiDto.CarImage.ContentType);
                    formData.Add(fileContent, "CarImage", uiDto.CarImage.FileName);
                }

                if (uiDto.LogoImage != null)
                {
                    var fileContent = new StreamContent(uiDto.LogoImage.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(uiDto.LogoImage.ContentType);
                    formData.Add(fileContent, "LogoImage", uiDto.LogoImage.FileName);
                }

                var response = await _httpClient.PutAsync("api/Banners", formData);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"API Hatası: {response.StatusCode} - {errorContent}");
                }
            }
            return View(uiDto);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var responseMessage = await _httpClient.DeleteAsync($"api/Banners/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var errorContent = await responseMessage.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"API Hatası: {responseMessage.StatusCode} - {errorContent}");
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/Banners/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var apiBanner = JsonConvert.DeserializeObject<DTOsLayer.WebApiDTO.BannerDTO.ResultBannerDTO>(jsonData);

                var uiBanner = _mapper.Map<ResultBannerDTO>(apiBanner);
                return View(uiBanner);
            }
            return View();
        }
    }
}