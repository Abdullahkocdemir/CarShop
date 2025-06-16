using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.BrandDTO;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : BaseEntityController
    {
        private readonly IBrandService _brandService;
        protected override string EntityTypeName => "Brand";

        public BrandsController(IBrandService brandService, EnhancedRabbitMQService rabbitMqService)
            : base(rabbitMqService)
        {
            _brandService = brandService;
        }

        [HttpPost]
        public IActionResult AddBrand(CreateBrandDTO dto)
        {
            try
            {
                var brand = new Brand
                {
                    BrandName = dto.BrandName
                };

                _brandService.BAdd(brand);
                PublishEntityCreated(brand);

                return Ok(new { Message = "Brand added and message published.", BrandId = brand.BrandId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddBrand: {ex.Message}");
                return StatusCode(500, "An error occurred while adding the brand.");
            }
        }

        [HttpPut]
        public IActionResult UpdateBrand(UpdateBrandDTO dto)
        {
            try
            {
                var existingBrand = _brandService.BGetById(dto.BrandId);
                if (existingBrand == null)
                    return NotFound("Brand not found.");

                existingBrand.BrandName = dto.BrandName;
                _brandService.BUpdate(existingBrand);
                PublishEntityUpdated(existingBrand); // 🎯 Otomatik mesaj gönderimi

                return Ok(new { Message = "Brand updated and message published.", BrandId = existingBrand.BrandId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateBrand: {ex.Message}");
                return StatusCode(500, "An error occurred while updating the brand.");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBrand(int id)
        {
            try
            {
                var brand = _brandService.BGetById(id);
                if (brand == null)
                    return NotFound("Brand not found.");

                _brandService.BDelete(brand);
                PublishEntityDeleted(brand); // 🎯 Otomatik mesaj gönderimi

                return Ok(new { Message = "Brand deleted and message published.", BrandId = id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteBrand: {ex.Message}");
                return StatusCode(500, "An error occurred while deleting the brand.");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetBrandById(int id)
        {
            try
            {
                var brand = _brandService.BGetById(id);
                if (brand == null)
                    return NotFound("Brand not found.");
                return Ok(brand);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBrandById: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving the brand.");
            }
        }

        [HttpGet]
        public IActionResult GetAllBrands()
        {
            try
            {
                var brands = _brandService.BGetListAll();
                return Ok(brands);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllBrands: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving brands.");
            }
        }
    }

}