using AutoMapper;
using DTOsLayer.WebUIDTO.ProductDTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using CarShop.WebUI.Models; 
using DTOsLayer.WebApiDTO.BrandDTO; 
using DTOsLayer.WebApiDTO.ModelDTO;
using DTOsLayer.WebApiDTO.ColorDTO;

namespace CarShop.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public ProductController(IHttpClientFactory httpClientFactory, IMapper mapper)
        {
            _httpClient = httpClientFactory.CreateClient("CarShopApiClient");
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? search,
            int? brandId,
            int? modelId,
            string? bodyStyle, 
            string? condition,
            string? transmission,
            string? engine, 
            int? colorId,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortBy,
            int showPerPage = 9 
        )
        {
            var viewModel = new ProductListViewModel();

            var productResponse = await _httpClient.GetAsync("api/Products");
            if (productResponse.IsSuccessStatusCode)
            {
                var jsonData = await productResponse.Content.ReadAsStringAsync();
                var apiProducts = JsonConvert.DeserializeObject<List<DTOsLayer.WebApiDTO.ProductDTOs.ResultProductDTO>>(jsonData);
                viewModel.Products = _mapper.Map<List<ResultProductDTO>>(apiProducts);
            }
            else
            {
                viewModel.Products = new List<ResultProductDTO>();
            }

            var brandResponse = await _httpClient.GetAsync("api/Brands");
            if (brandResponse.IsSuccessStatusCode)
            {
                var jsonData = await brandResponse.Content.ReadAsStringAsync();
                var apiBrands = JsonConvert.DeserializeObject<List<DTOsLayer.WebApiDTO.BrandDTO.ResultBrandDTO>>(jsonData);
                viewModel.Brands = _mapper.Map<List<DTOsLayer.WebUIDTO.BrandDTO.ResultBrandDTO>>(apiBrands);
            }

            var modelResponse = await _httpClient.GetAsync("api/Models");
            if (modelResponse.IsSuccessStatusCode)
            {
                var jsonData = await modelResponse.Content.ReadAsStringAsync();
                var apiModels = JsonConvert.DeserializeObject<List<DTOsLayer.WebApiDTO.ModelDTO.ResultModelDTO>>(jsonData);
                viewModel.Models = _mapper.Map<List<DTOsLayer.WebUIDTO.ModelsDTO.ResultModelDTO>>(apiModels);
            }

            var colorResponse = await _httpClient.GetAsync("api/Colors");
            if (colorResponse.IsSuccessStatusCode)
            {
                var jsonData = await colorResponse.Content.ReadAsStringAsync();
                var apiColors = JsonConvert.DeserializeObject<List<DTOsLayer.WebApiDTO.ColorDTO.ResultColorDTO>>(jsonData);
                viewModel.Colors = _mapper.Map<List<DTOsLayer.WebUIDTO.ColorDTO.ResultColorDTO>>(apiColors);
            }

            var filteredProducts = viewModel.Products.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                filteredProducts = filteredProducts.Where(p =>
                    p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (p.Description != null && p.Description.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    p.BrandName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.ModelName.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (brandId.HasValue && brandId.Value > 0)
            {
                filteredProducts = filteredProducts.Where(p => p.BrandId == brandId.Value);
            }

            if (modelId.HasValue && modelId.Value > 0)
            {
                filteredProducts = filteredProducts.Where(p => p.ModelId == modelId.Value);
            }

            if (!string.IsNullOrEmpty(condition) && condition != "Condition")
            {
                filteredProducts = filteredProducts.Where(p => p.Condition == condition);
            }

            if (!string.IsNullOrEmpty(transmission) && transmission != "Transmisson")
            {
                filteredProducts = filteredProducts.Where(p => p.Transmission == transmission);
            }

            if (!string.IsNullOrEmpty(engine) && engine != "Engine")
            {
                filteredProducts = filteredProducts.Where(p => p.EngineSize == engine);
            }

            if (colorId.HasValue && colorId.Value > 0)
            {
                filteredProducts = filteredProducts.Where(p => p.ColorId == colorId.Value);
            }

            if (minPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price <= maxPrice.Value);
            }

            switch (sortBy)
            {
                case "Price: Highest Fist":
                    filteredProducts = filteredProducts.OrderByDescending(p => p.Price);
                    break;
                case "Price: Lowest Fist":
                    filteredProducts = filteredProducts.OrderBy(p => p.Price);
                    break;
                default:
                    filteredProducts = filteredProducts.OrderByDescending(p => p.CreatedDate); 
                    break;
            }

            viewModel.Products = filteredProducts.Take(showPerPage).ToList();

            ViewBag.CurrentSearch = search;
            ViewBag.CurrentBrandId = brandId;
            ViewBag.CurrentModelId = modelId;
            ViewBag.CurrentCondition = condition;
            ViewBag.CurrentTransmission = transmission;
            ViewBag.CurrentEngine = engine;
            ViewBag.CurrentColorId = colorId;
            ViewBag.CurrentMinPrice = minPrice;
            ViewBag.CurrentMaxPrice = maxPrice;
            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentShowPerPage = showPerPage;

            return View(viewModel);
        }
    }
}