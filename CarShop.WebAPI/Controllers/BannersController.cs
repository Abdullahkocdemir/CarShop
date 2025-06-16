using AutoMapper; // AutoMapper için using
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.BannerDTO;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannersController : BaseEntityController
    {
        private readonly IBannerService _bannerService;
        private readonly IMapper _mapper; // AutoMapper'ı enjekte edeceğiz
        protected override string EntityTypeName => "Banner";

        public BannersController(IBannerService bannerService, EnhancedRabbitMQService rabbitMqService, IMapper mapper)
            : base(rabbitMqService)
        {
            _bannerService = bannerService;
            _mapper = mapper; // AutoMapper'ı constructor'da al
        }

        // --- CRUD Operasyonları ---

        [HttpPost]
        public IActionResult AddBanner(CreateBannerDTO dto)
        {
            try
            {
                // DTO'dan Entity'ye AutoMapper ile mapleme
                var banner = _mapper.Map<Banner>(dto);

                _bannerService.BAdd(banner);
                PublishEntityCreated(banner);

                return Ok(new { Message = "Banner added and message published.", BannerId = banner.BannerId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddBanner: {ex.Message}");
                return StatusCode(500, "An error occurred while adding the banner.");
            }
        }

        [HttpPut]
        public IActionResult UpdateBanner(UpdateBannerDTO dto)
        {
            try
            {
                var existingBanner = _bannerService.BGetById(dto.BannerId);
                if (existingBanner == null)
                    return NotFound("Banner not found.");

                // DTO'daki değişiklikleri mevcut Entity üzerine mapleme
                _mapper.Map(dto, existingBanner);

                _bannerService.BUpdate(existingBanner);
                PublishEntityUpdated(existingBanner);

                return Ok(new { Message = "Banner updated and message published.", BannerId = existingBanner.BannerId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateBanner: {ex.Message}");
                return StatusCode(500, "An error occurred while updating the banner.");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBanner(int id)
        {
            try
            {
                var banner = _bannerService.BGetById(id);
                if (banner == null)
                    return NotFound("Banner not found.");

                _bannerService.BDelete(banner);
                PublishEntityDeleted(banner);

                return Ok(new { Message = "Banner deleted and message published.", BannerId = id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteBanner: {ex.Message}");
                return StatusCode(500, "An error occurred while deleting the banner.");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetBannerById(int id)
        {
            try
            {
                var banner = _bannerService.BGetById(id);
                if (banner == null)
                    return NotFound("Banner not found.");

                // Eğer entity'yi bir DTO'ya çevirip döndürmek istersen:
                // var resultDto = _mapper.Map<ResultBannerDTO>(banner);
                // return Ok(resultDto);
                // Şimdilik direkt entity dönüyor:
                return Ok(banner);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetBannerById: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving the banner.");
            }
        }

        [HttpGet]
        public IActionResult GetAllBanners()
        {
            try
            {
                var banners = _bannerService.BGetListAll();
                return Ok(banners);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllBanners: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving banners.");
            }
        }
    }
}