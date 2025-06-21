using AutoMapper;
using BusinessLayer.Abstract; // Assuming IPartnerService is here
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.PartnerDTO; // Your new Partner DTOs
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnersController : BaseEntityController
    {
        private readonly IPartnerService _partnerService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        protected override string EntityTypeName => "Partner";

        public PartnersController(IPartnerService partnerService, IMapper mapper, EnhancedRabbitMQService rabbitMqService, IWebHostEnvironment webHostEnvironment)
            : base(rabbitMqService)
        {
            _partnerService = partnerService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public IActionResult GetListAllPartners()
        {
            var partners = _partnerService.BGetListAll();
            var partnerDtos = _mapper.Map<List<ResultPartnerDTO>>(partners);
            return Ok(partnerDtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetPartnerById(int id)
        {
            var partner = _partnerService.BGetById(id);
            if (partner == null)
            {
                return NotFound($"ID'si {id} olan iş ortağı bulunamadı.");
            }
            var partnerDto = _mapper.Map<GetByIdPartnerDTO>(partner);
            return Ok(partnerDto);
        }
        [HttpPost]
        public async Task<IActionResult> CreatePartner([FromForm] CreatePartnerDTO dto)
        {
            var partner = _mapper.Map<Partner>(dto);


            if (dto.ImageFile != null)
            {
                partner.ImageUrl = await SaveImage(dto.ImageFile);
            }
            else
            {
                return BadRequest("İş ortağı oluşturmak için bir resim dosyası gereklidir.");
            }

            _partnerService.BAdd(partner);
            PublishEntityCreated(partner);

            return Ok(new { Message = "İş ortağı başarıyla eklendi ve mesaj gönderildi.", PartnerId = partner.PartnerId, ImageUrl = partner.ImageUrl });
        }
        [HttpPut]
        public async Task<IActionResult> UpdatePartner([FromForm] UpdatePartnerDTO dto)
        {
            var existingPartner = _partnerService.BGetById(dto.PartnerId);
            if (existingPartner == null)
            {
                return NotFound($"ID'si {dto.PartnerId} olan iş ortağı bulunamadı.");
            }

            _mapper.Map(dto, existingPartner);

            if (dto.ImageFile != null)
            {
                if (!string.IsNullOrEmpty(existingPartner.ImageUrl))
                {
                    DeleteImage(existingPartner.ImageUrl);
                }
                existingPartner.ImageUrl = await SaveImage(dto.ImageFile);
            }
            else if (!string.IsNullOrEmpty(dto.ExistingImageUrl))
            {
                existingPartner.ImageUrl = dto.ExistingImageUrl;
            }
            else
            {

                if (!string.IsNullOrEmpty(existingPartner.ImageUrl))
                {
                    DeleteImage(existingPartner.ImageUrl);
                }
                existingPartner.ImageUrl = string.Empty;
            }

            _partnerService.BUpdate(existingPartner);
            PublishEntityUpdated(existingPartner);

            return Ok(new { Message = "İş ortağı başarıyla güncellendi ve mesaj yayınlandı.", PartnerId = existingPartner.PartnerId, ImageUrl = existingPartner.ImageUrl });
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePartner(int id)
        {
            var partnerToDelete = _partnerService.BGetById(id);
            if (partnerToDelete == null)
            {
                return NotFound($"ID'si {id} olan iş ortağı bulunamadı.");
            }

            // Delete the associated image file
            if (!string.IsNullOrEmpty(partnerToDelete.ImageUrl))
            {
                DeleteImage(partnerToDelete.ImageUrl);
            }

            _partnerService.BDelete(partnerToDelete);
            PublishEntityDeleted(partnerToDelete);

            return Ok(new { Message = "İş ortağı başarıyla silindi ve mesaj yayınlandı.", PartnerId = id });
        }

        private async Task<string> SaveImage(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "partners");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return $"/partners/{uniqueFileName}";
        }

        private void DeleteImage(string imageUrl)
        {
            var fileName = Path.GetFileName(imageUrl);
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "partners", fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}