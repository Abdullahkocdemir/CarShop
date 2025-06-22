using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.BannerDTO;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting; 
using System;
using System.Collections.Generic;
using System.IO;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannersController : BaseEntityController
    {
        private readonly IBannerService _bannerService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment; 
        protected override string EntityTypeName => "Banner";

        public BannersController(IBannerService bannerService, EnhancedRabbitMQService rabbitMqService, IMapper mapper, IWebHostEnvironment webHostEnvironment)
            : base(rabbitMqService)
        {
            _bannerService = bannerService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment; 
        }

        [HttpGet]
        public IActionResult GetListAllBanner()
        {
            var values = _bannerService.BGetListAll();
            var result = values.Select(banner => new ResultBannerDTO
            {
                BannerId = banner.BannerId,
                SmallTitle = banner.SmallTitle,
                SubTitle = banner.SubTitle,
                CarImageUrl = !string.IsNullOrEmpty(banner.CarImageUrl) ? $"{Request.Scheme}://{Request.Host}/banner/{banner.CarImageUrl}" : string.Empty,
                CarModel = banner.CarModel,
                Month = banner.Month,
                LogoImageUrl = !string.IsNullOrEmpty(banner.LogoImageUrl) ? $"{Request.Scheme}://{Request.Host}/banner/{banner.LogoImageUrl}" : string.Empty,
                Price = banner.Price
            }).ToList();

            return Ok(result);
        }

        [HttpPost]
        [Consumes("multipart/form-data")] 
        public async Task<IActionResult> CreateBanner([FromForm] CreateBannerDTO dto)
        {
            var banner = _mapper.Map<Banner>(dto);

            if (dto.CarImage != null)
            {
                banner.CarImageUrl = await SaveImage(dto.CarImage);
            }

            if (dto.LogoImage != null)
            {
                banner.LogoImageUrl = await SaveImage(dto.LogoImage);
            }

            _bannerService.BAdd(banner);
            PublishEntityCreated(banner);
            return Ok(new { Message = "Banner added and message published.", BannerId = banner.BannerId });
        }

        [HttpPut]
        [Consumes("multipart/form-data")] 
        public async Task<IActionResult> UpdateBanner([FromForm] UpdateBannerDTO dto)
        {
            var existingBanner = _bannerService.BGetById(dto.BannerId);

            if (existingBanner == null)
            {
                return NotFound($"Banner with ID {dto.BannerId} not found.");
            }

            _mapper.Map(dto, existingBanner); 

            if (dto.CarImage != null)
            {
                if (!string.IsNullOrEmpty(existingBanner.CarImageUrl))
                {
                    DeleteImage(existingBanner.CarImageUrl);
                }
                existingBanner.CarImageUrl = await SaveImage(dto.CarImage);
            }

            if (dto.LogoImage != null)
            {
                if (!string.IsNullOrEmpty(existingBanner.LogoImageUrl))
                {
                    DeleteImage(existingBanner.LogoImageUrl);
                }
                existingBanner.LogoImageUrl = await SaveImage(dto.LogoImage);
            }

            _bannerService.BUpdate(existingBanner);
            PublishEntityUpdated(existingBanner);

            return Ok(new { Message = "Afiş Güncellendi ve Mesaj Yayınlandı.", BannerId = existingBanner.BannerId });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBanner(int id)
        {
            var banner = _bannerService.BGetById(id);
            if (banner == null)
            {
                return NotFound($"Banner with ID {id} not found.");
            }

            if (!string.IsNullOrEmpty(banner.CarImageUrl))
            {
                DeleteImage(banner.CarImageUrl);
            }
            if (!string.IsNullOrEmpty(banner.LogoImageUrl))
            {
                DeleteImage(banner.LogoImageUrl);
            }

            _bannerService.BDelete(banner);
            PublishEntityDeleted(banner);
            return Ok(new { Message = "Afiş Silindi Ve Yayınlandı", BannerId = id });
        }

        [HttpGet("{id}")]
        public IActionResult GetByIdBanner(int id)
        {
            var value = _bannerService.BGetById(id);
            if (value == null)
            {
                return NotFound($"Banner with ID {id} not found.");
            }

            var result = new ResultBannerDTO
            {
                BannerId = value.BannerId,
                SmallTitle = value.SmallTitle,
                SubTitle = value.SubTitle,
                CarImageUrl = !string.IsNullOrEmpty(value.CarImageUrl) ? $"{Request.Scheme}://{Request.Host}/banner/{value.CarImageUrl}" : string.Empty,
                CarModel = value.CarModel,
                Month = value.Month,
                LogoImageUrl = !string.IsNullOrEmpty(value.LogoImageUrl) ? $"{Request.Scheme}://{Request.Host}/banner/{value.LogoImageUrl}" : string.Empty,
                Price = value.Price
            };
            return Ok(result);
        }


        private async Task<string> SaveImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return string.Empty;
            }

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "banner");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return uniqueFileName;
        }

        private void DeleteImage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "banner", fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}