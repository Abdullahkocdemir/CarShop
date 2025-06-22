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
using CarShop.WebUI.Models;

namespace CarShop.WebUI.Controllers
{
    public class AdminProductController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public AdminProductController(IHttpClientFactory httpClientFactory, IMapper mapper)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("api/Products");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var apiProducts = JsonConvert.DeserializeObject<List<WebApiDTOs.ResultProductDTO>>(jsonData);
                var uiProducts = _mapper.Map<List<ResultProductDTO>>(apiProducts);
                return View(uiProducts);
            }
            return View(new List<ResultProductDTO>());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateProductViewModel();
            await LoadDropdownsToViewModel(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Consumes("multipart/form-data")] 
        public async Task<IActionResult> Create(CreateProductViewModel viewModel) 
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsToViewModel(viewModel);
                return View(viewModel);
            }

            var apiDto = _mapper.Map<WebApiDTOs.CreateProductDTO>(viewModel.Product);

            using (var formData = new MultipartFormDataContent())
            {
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

                if (viewModel.Product.ImageFiles != null && viewModel.Product.ImageFiles.Any())
                {
                    for (int i = 0; i < viewModel.Product.ImageFiles.Count; i++)
                    {
                        var file = viewModel.Product.ImageFiles[i];
                        if (file != null && file.Length > 0)
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
                    TempData["ErrorMessage"] = $"Ürün eklenirken hata oluştu: {errorContent}"; 
                }
            }

            await LoadDropdownsToViewModel(viewModel);
            return View(viewModel);
        }
        private async Task LoadDropdowns()
        {
            try
            {
                var brandResponse = await _httpClient.GetAsync("api/Brands");
                if (brandResponse.IsSuccessStatusCode)
                {
                    var brands = JsonConvert.DeserializeObject<List<ResultBrandDTO>>(await brandResponse.Content.ReadAsStringAsync());
                    ViewBag.Brands = new SelectList(brands ?? new List<ResultBrandDTO>(), "BrandId", "BrandName"); 
                }
                else { ViewBag.Brands = new SelectList(new List<dynamic>(), "Value", "Text"); }

                var colorResponse = await _httpClient.GetAsync("api/Colors");
                if (colorResponse.IsSuccessStatusCode)
                {
                    var colors = JsonConvert.DeserializeObject<List<ResultColorDTO>>(await colorResponse.Content.ReadAsStringAsync());
                    ViewBag.Colors = new SelectList(colors ?? new List<ResultColorDTO>(), "ColorId", "Name");
                }
                else { ViewBag.Colors = new SelectList(new List<dynamic>(), "Value", "Text"); }

                var modelResponse = await _httpClient.GetAsync("api/Models");
                if (modelResponse.IsSuccessStatusCode)
                {
                    var models = JsonConvert.DeserializeObject<List<ResultModelDTO>>(await modelResponse.Content.ReadAsStringAsync());
                    ViewBag.Models = new SelectList(models ?? new List<ResultModelDTO>(), "ModelId", "Name");
                }
                else { ViewBag.Models = new SelectList(new List<dynamic>(), "Value", "Text"); }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadDropdowns genel hata: {ex.Message}");
                ViewBag.Brands = new SelectList(new List<dynamic>(), "Value", "Text");
                ViewBag.Colors = new SelectList(new List<dynamic>(), "Value", "Text");
                ViewBag.Models = new SelectList(new List<dynamic>(), "Value", "Text");
            }
        }

        private async Task LoadDropdownsToViewModel(CreateProductViewModel viewModel)
        {
            try
            {
                var brandResponse = await _httpClient.GetAsync("api/Brands");
                if (brandResponse.IsSuccessStatusCode)
                {
                    var brandJson = await brandResponse.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(brandJson))
                    {
                        var brands = JsonConvert.DeserializeObject<List<ResultBrandDTO>>(brandJson);
                        viewModel.Brands = brands?.Select(b => new SelectListItem
                        {
                            Value = b.BrandId.ToString(),
                            Text = b.BrandName 
                        }).ToList() ?? new List<SelectListItem>();
                    }
                }

                var colorResponse = await _httpClient.GetAsync("api/Colors");
                if (colorResponse.IsSuccessStatusCode)
                {
                    var colorJson = await colorResponse.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(colorJson))
                    {
                        var colors = JsonConvert.DeserializeObject<List<ResultColorDTO>>(colorJson);
                        viewModel.Colors = colors?.Select(c => new SelectListItem
                        {
                            Value = c.ColorId.ToString(),
                            Text = c.Name
                        }).ToList() ?? new List<SelectListItem>();
                    }
                }

                var modelResponse = await _httpClient.GetAsync("api/Models");
                if (modelResponse.IsSuccessStatusCode)
                {
                    var modelJson = await modelResponse.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(modelJson))
                    {
                        var models = JsonConvert.DeserializeObject<List<ResultModelDTO>>(modelJson);
                        viewModel.Models = models?.Select(m => new SelectListItem
                        {
                            Value = m.ModelId.ToString(),
                            Text = m.Name
                        }).ToList() ?? new List<SelectListItem>();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadDropdownsToViewModel genel hata: {ex.Message}");
                viewModel.Brands = new List<SelectListItem>();
                viewModel.Colors = new List<SelectListItem>();
                viewModel.Models = new List<SelectListItem>();
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"api/Products/{id}");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var apiProduct = JsonConvert.DeserializeObject<WebApiDTOs.GetByIdProductDTO>(jsonData);

                var uiDto = _mapper.Map<UpdateProductDTO>(apiProduct);
                await LoadDropdowns(); 
                return View(uiDto);
            }
            TempData["ErrorMessage"] = "Ürün bulunamadı veya bir hata oluştu.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Edit(UpdateProductDTO uiDto)
        {

            var apiDto = _mapper.Map<WebApiDTOs.UpdateProductDTO>(uiDto);

            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent(uiDto.ProductId.ToString()), "ProductId");

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


                for (int i = 0; i < uiDto.Images.Count; i++)
                {
                    var image = uiDto.Images[i];
                    formData.Add(new StringContent(image.Id.ToString()), $"Images[{i}].Id");
                    formData.Add(new StringContent(image.ShouldDelete.ToString()), $"Images[{i}].ShouldDelete");
                    formData.Add(new StringContent(image.IsMainImage.ToString()), $"Images[{i}].IsMainImage");
                    formData.Add(new StringContent(image.Order.ToString()), $"Images[{i}].Order");

                    if (image.ImageFile != null)
                    {
                        var fileContent = new StreamContent(image.ImageFile.OpenReadStream());
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(image.ImageFile.ContentType);
                        formData.Add(fileContent, $"Images[{i}].ImageFile", image.ImageFile.FileName);
                    }
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
            await LoadDropdowns(); 
            return View(uiDto);
        }

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

    }
}