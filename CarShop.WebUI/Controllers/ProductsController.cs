using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using AutoMapper;
using DTOsLayer.WebUIDTO.ProductDTO; 
using WebApiDTOs = DTOsLayer.WebApiDTO.ProductDTOs; 
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc.Rendering; 
using DTOsLayer.WebUIDTO.BrandDTO;
using DTOsLayer.WebApiDTO.ColorDTO;
using DTOsLayer.WebApiDTO.ModelDTO;

namespace CarShop.WebUI.Controllers
{
    public class ProductsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public ProductsController(IHttpClientFactory httpClientFactory, IMapper mapper)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient"); // Program.cs'de tanımladığınız isim
            _mapper = mapper;
        }

        /// <summary>
        /// Tüm ürünleri listeler.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/Products"); // API endpoint'iniz
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var apiProducts = JsonConvert.DeserializeObject<List<WebApiDTOs.ResultProductDTO>>(jsonData);
                var uiProducts = _mapper.Map<List<ResultProductDTO>>(apiProducts);
                return View(uiProducts);
            }
            // Hata durumunda boş liste veya hata mesajı gösterebiliriz
            return View(new List<ResultProductDTO>());
        }

        /// <summary>
        /// Yeni ürün oluşturma formunu gösterir.
        /// Ayrıca Brand, Color, Model seçeneklerini doldurmak için API'den verileri çeker.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns(); // Brand, Color, Model seçeneklerini yükle
            return View();
        }

        /// <summary>
        /// Yeni ürün oluşturma işlemini yapar.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Consumes("multipart/form-data")] // Dosya yüklemeleri için
        public async Task<IActionResult> Create(CreateProductDTO uiDto)
        {
            // UI DTO'dan API DTO'ya eşleme
            var apiDto = _mapper.Map<WebApiDTOs.CreateProductDTO>(uiDto);

            // MultipartFormDataContent oluştur
            using (var formData = new MultipartFormDataContent())
            {
                // Metin tabanlı alanları ekle
                formData.Add(new StringContent(apiDto.Name ?? string.Empty), "Name");
                formData.Add(new StringContent(apiDto.ColorId.ToString()), "ColorId");
                formData.Add(new StringContent(apiDto.BrandId.ToString()), "BrandId");
                formData.Add(new StringContent(apiDto.ModelId.ToString()), "ModelId");
                formData.Add(new StringContent(apiDto.Kilometer ?? string.Empty), "Kilometer");
                formData.Add(new StringContent(apiDto.Year.ToString()), "Year");
                formData.Add(new StringContent(apiDto.Price.ToString(System.Globalization.CultureInfo.InvariantCulture)), "Price"); // Kültüre duyarsız ondalık nokta
                formData.Add(new StringContent(apiDto.EngineSize ?? string.Empty), "EngineSize");
                formData.Add(new StringContent(apiDto.FuelType ?? string.Empty), "FuelType");
                formData.Add(new StringContent(apiDto.Transmission ?? string.Empty), "Transmission");
                formData.Add(new StringContent(apiDto.Horsepower?.ToString() ?? string.Empty), "Horsepower");
                formData.Add(new StringContent(apiDto.DriveType ?? string.Empty), "DriveType");
                formData.Add(new StringContent(apiDto.DoorCount?.ToString() ?? string.Empty), "DoorCount");
                formData.Add(new StringContent(apiDto.SeatCount?.ToString() ?? string.Empty), "SeatCount");
                formData.Add(new StringContent(apiDto.Condition ?? string.Empty), "Condition");
                formData.Add(new StringContent(apiDto.HasAirbag.ToString()), "HasAirbag");
                formData.Add(new StringContent(apiDto.HasABS.ToString()), "HasABS");
                formData.Add(new StringContent(apiDto.HasESP.ToString()), "HasESP");
                formData.Add(new StringContent(apiDto.HasAirConditioning.ToString()), "HasAirConditioning");
                formData.Add(new StringContent(apiDto.HasSunroof.ToString()), "HasSunroof");
                formData.Add(new StringContent(apiDto.HasLeatherSeats.ToString()), "HasLeatherSeats");
                formData.Add(new StringContent(apiDto.HasNavigationSystem.ToString()), "HasNavigationSystem");
                formData.Add(new StringContent(apiDto.HasParkingSensors.ToString()), "HasParkingSensors");
                formData.Add(new StringContent(apiDto.HasBackupCamera.ToString()), "HasBackupCamera");
                formData.Add(new StringContent(apiDto.HasCruiseControl.ToString()), "HasCruiseControl");
                formData.Add(new StringContent(apiDto.Description ?? string.Empty), "Description");
                formData.Add(new StringContent(apiDto.Features ?? string.Empty), "Features");
                formData.Add(new StringContent(apiDto.DamageHistory ?? string.Empty), "DamageHistory");
                formData.Add(new StringContent(apiDto.City ?? string.Empty), "City");
                formData.Add(new StringContent(apiDto.District ?? string.Empty), "District");
                formData.Add(new StringContent(apiDto.SellerType ?? string.Empty), "SellerType");

                // Resim dosyalarını ekle
                if (uiDto.ImageFiles != null && uiDto.ImageFiles.Any())
                {
                    for (int i = 0; i < uiDto.ImageFiles.Count; i++)
                    {
                        var file = uiDto.ImageFiles[i];
                        if (file != null && file.Length > 0) // Dosya null veya boş değilse ekle
                        {
                            var fileContent = new StreamContent(file.OpenReadStream());
                            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                            formData.Add(fileContent, $"ImageFiles[{i}]", file.FileName);
                        }
                    }
                }

                var response = await _httpClient.PostAsync("api/Products", formData);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Ürün başarıyla eklendi!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"API Hatası: {response.StatusCode} - {errorContent}");
                }
            }
            await LoadDropdowns(); // Hata durumunda dropdown'ları tekrar yükle
            return View(uiDto);
        }

        /// <summary>
        /// Ürün güncelleme formunu gösterir.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"api/Products/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var apiProduct = JsonConvert.DeserializeObject<WebApiDTOs.GetByIdProductDTO>(jsonData);

                // AutoMapper'ın doğru eşleme yapmasını sağlamak için doğrudan kullanıyoruz
                var uiDto = _mapper.Map<UpdateProductDTO>(apiProduct);

                // LoadDropdowns() metodunu burada çağırın
                await LoadDropdowns();
                return View(uiDto);
            }
            TempData["ErrorMessage"] = "Ürün bulunamadı veya bir hata oluştu.";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Ürün güncelleme işlemini yapar.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Consumes("multipart/form-data")] // Dosya yüklemeleri için
        public async Task<IActionResult> Edit(UpdateProductDTO uiDto)
        {
            // ModelState geçerli değilse, formu dropdown'larla birlikte yeniden göster
            // if (!ModelState.IsValid) // Model doğrulaması yapıyorsanız bu kontrolü ekleyebilirsiniz
            // {
            //     await LoadDropdowns();
            //     return View(uiDto);
            // }

            var apiDto = _mapper.Map<WebApiDTOs.UpdateProductDTO>(uiDto);

            using (var formData = new MultipartFormDataContent())
            {
                // ProductId'yi FormData'ya ekle
                formData.Add(new StringContent(uiDto.ProductId.ToString()), "ProductId");

                // Metin tabanlı alanları ekle
                formData.Add(new StringContent(apiDto.Name ?? string.Empty), "Name");
                formData.Add(new StringContent(apiDto.ColorId.ToString()), "ColorId");
                formData.Add(new StringContent(apiDto.BrandId.ToString()), "BrandId");
                formData.Add(new StringContent(apiDto.ModelId.ToString()), "ModelId");
                formData.Add(new StringContent(apiDto.Kilometer ?? string.Empty), "Kilometer");
                formData.Add(new StringContent(apiDto.Year.ToString()), "Year");
                formData.Add(new StringContent(apiDto.Price.ToString(System.Globalization.CultureInfo.InvariantCulture)), "Price");
                formData.Add(new StringContent(apiDto.EngineSize ?? string.Empty), "EngineSize");
                formData.Add(new StringContent(apiDto.FuelType ?? string.Empty), "FuelType");
                formData.Add(new StringContent(apiDto.Transmission ?? string.Empty), "Transmission");
                formData.Add(new StringContent(apiDto.Horsepower?.ToString() ?? string.Empty), "Horsepower");
                formData.Add(new StringContent(apiDto.DriveType ?? string.Empty), "DriveType");
                formData.Add(new StringContent(apiDto.DoorCount?.ToString() ?? string.Empty), "DoorCount");
                formData.Add(new StringContent(apiDto.SeatCount?.ToString() ?? string.Empty), "SeatCount");
                formData.Add(new StringContent(apiDto.Condition ?? string.Empty), "Condition");
                formData.Add(new StringContent(apiDto.HasAirbag.ToString()), "HasAirbag");
                formData.Add(new StringContent(apiDto.HasABS.ToString()), "HasABS");
                formData.Add(new StringContent(apiDto.HasESP.ToString()), "HasESP");
                formData.Add(new StringContent(apiDto.HasAirConditioning.ToString()), "HasAirConditioning");
                formData.Add(new StringContent(apiDto.HasSunroof.ToString()), "HasSunroof");
                formData.Add(new StringContent(apiDto.HasLeatherSeats.ToString()), "HasLeatherSeats");
                formData.Add(new StringContent(apiDto.HasNavigationSystem.ToString()), "HasNavigationSystem");
                formData.Add(new StringContent(apiDto.HasParkingSensors.ToString()), "HasParkingSensors");
                formData.Add(new StringContent(apiDto.HasBackupCamera.ToString()), "HasBackupCamera");
                formData.Add(new StringContent(apiDto.HasCruiseControl.ToString()), "HasCruiseControl");
                formData.Add(new StringContent(apiDto.Description ?? string.Empty), "Description");
                formData.Add(new StringContent(apiDto.Features ?? string.Empty), "Features");
                formData.Add(new StringContent(apiDto.DamageHistory ?? string.Empty), "DamageHistory");
                formData.Add(new StringContent(apiDto.City ?? string.Empty), "City");
                formData.Add(new StringContent(apiDto.District ?? string.Empty), "District");
                formData.Add(new StringContent(apiDto.SellerType ?? string.Empty), "SellerType");
                formData.Add(new StringContent(apiDto.IsActive.ToString()), "IsActive");
                formData.Add(new StringContent(apiDto.IsPopular.ToString()), "IsPopular");


                // Resimleri işleme:
                for (int i = 0; i < uiDto.Images.Count; i++)
                {
                    var image = uiDto.Images[i];
                    // Her zaman Id'yi ve ShouldDelete'i gönderin ki API tarafında doğru işlem yapılabilsin
                    formData.Add(new StringContent(image.Id.ToString()), $"Images[{i}].Id");
                    formData.Add(new StringContent(image.ShouldDelete.ToString()), $"Images[{i}].ShouldDelete");
                    formData.Add(new StringContent(image.IsMainImage.ToString()), $"Images[{i}].IsMainImage");
                    formData.Add(new StringContent(image.Order.ToString()), $"Images[{i}].Order");

                    // Yalnızca yeni yüklenen veya güncellenen dosyaları ekle
                    if (image.ImageFile != null)
                    {
                        var fileContent = new StreamContent(image.ImageFile.OpenReadStream());
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(image.ImageFile.ContentType);
                        formData.Add(fileContent, $"Images[{i}].ImageFile", image.ImageFile.FileName);
                    }
                    // Eğer resim silinmiyorsa ve yeni bir dosya yüklenmiyorsa, mevcut ImageUrl'i de gönderin
                    // Bu, API tarafında mevcut resmin tanınmasına yardımcı olabilir (Opsiyonel ama güvenli)
                    else if (!image.ShouldDelete && !string.IsNullOrEmpty(image.ImageUrl))
                    {
                        formData.Add(new StringContent(image.ImageUrl), $"Images[{i}].ImageUrl");
                    }
                }


                var response = await _httpClient.PutAsync("api/Products", formData);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Ürün başarıyla güncellendi!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"API Hatası: {response.StatusCode} - {errorContent}");
                }
            }
            await LoadDropdowns(); // Hata durumunda dropdown'ları tekrar yükle
            return View(uiDto);
        }

        /// <summary>
        /// Ürünü silme işlemini yapar.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var responseMessage = await _httpClient.DeleteAsync($"api/Products/{id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Ürün başarıyla silindi!";
                return RedirectToAction("Index");
            }
            else
            {
                var errorContent = await responseMessage.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"API Hatası: {responseMessage.StatusCode} - {errorContent}");
                TempData["ErrorMessage"] = $"Ürün silinirken bir hata oluştu: {errorContent}";
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Ürün detaylarını gösterir.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"api/Products/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var apiProduct = JsonConvert.DeserializeObject<WebApiDTOs.GetByIdProductDTO>(jsonData);

                var uiProduct = _mapper.Map<GetByIdProductDTO>(apiProduct);
                return View(uiProduct);
            }
            TempData["ErrorMessage"] = "Ürün bulunamadı veya bir hata oluştu.";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Brand, Color ve Model dropdown'ları için verileri API'den çeker ve ViewBag'e atar.
        /// </summary>
        private async Task LoadDropdowns()
        {
            // Markaları çek
            var brandResponse = await _httpClient.GetAsync("api/Brands");
            if (brandResponse.IsSuccessStatusCode)
            {
                var brandJson = await brandResponse.Content.ReadAsStringAsync();
                var brands = JsonConvert.DeserializeObject<List<ResultBrandDTO>>(brandJson); // Doğru DTO tipini kullanın
                ViewBag.Brands = new SelectList(brands, "BrandId", "Name");
            }
            else
            {
                ViewBag.Brands = new List<SelectListItem>();
                ModelState.AddModelError("", "Markalar yüklenirken hata oluştu.");
            }

            // Renkleri çek
            var colorResponse = await _httpClient.GetAsync("api/Colors");
            if (colorResponse.IsSuccessStatusCode)
            {
                var colorJson = await colorResponse.Content.ReadAsStringAsync();
                var colors = JsonConvert.DeserializeObject<List<ResultColorDTO>>(colorJson); // Doğru DTO tipini kullanın
                ViewBag.Colors = new SelectList(colors, "ColorId", "Name");
            }
            else
            {
                ViewBag.Colors = new List<SelectListItem>();
                ModelState.AddModelError("", "Renkler yüklenirken hata oluştu.");
            }

            // Modelleri çek
            var modelResponse = await _httpClient.GetAsync("api/Models");
            if (modelResponse.IsSuccessStatusCode)
            {
                var modelJson = await modelResponse.Content.ReadAsStringAsync();
                var models = JsonConvert.DeserializeObject<List<ResultModelDTO>>(modelJson); // Doğru DTO tipini kullanın
                ViewBag.Models = new SelectList(models, "ModelId", "Name");
            }
            else
            {
                ViewBag.Models = new List<SelectListItem>();
                ModelState.AddModelError("", "Modeller yüklenirken hata oluştu.");
            }
        }
    }
}