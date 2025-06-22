using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.PartnerDTO;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var partner = _mapper.Map<Partner>(dto);
            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                partner.ImageUrl = await SaveImage(dto.ImageFile, "partners");
            }
            else
            {
                return BadRequest("İş ortağı oluşturmak için bir resim dosyası gereklidir.");
            }

            _partnerService.BAdd(partner);
            PublishEntityCreated(partner);

            return StatusCode(201, new { Message = "İş ortağı başarıyla eklendi ve mesaj gönderildi.", PartnerId = partner.PartnerId, ImageUrl = partner.ImageUrl });
        }
        [HttpPut]
        public async Task<IActionResult> UpdatePartner([FromForm] UpdatePartnerDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingPartner = _partnerService.BGetById(dto.PartnerId);
            if (existingPartner == null)
            {
                return NotFound($"ID'si {dto.PartnerId} olan iş ortağı bulunamadı.");
            }

            _mapper.Map(dto, existingPartner);

            if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingPartner.ImageUrl))
                {
                    DeleteImage(existingPartner.ImageUrl, "partners");
                }
                existingPartner.ImageUrl = await SaveImage(dto.ImageFile, "partners");
            }
            else if (!string.IsNullOrEmpty(dto.ExistingImageUrl))
            {
                existingPartner.ImageUrl = dto.ExistingImageUrl;
            }
            else
            {
                if (!string.IsNullOrEmpty(existingPartner.ImageUrl))
                {
                    DeleteImage(existingPartner.ImageUrl, "partners");
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

            // İlişkili resim dosyasını sil
            if (!string.IsNullOrEmpty(partnerToDelete.ImageUrl))
            {
                DeleteImage(partnerToDelete.ImageUrl, "partners");
            }

            _partnerService.BDelete(partnerToDelete);
            PublishEntityDeleted(partnerToDelete);

            return Ok(new { Message = "İş ortağı başarıyla silindi ve mesaj yayınlandı.", PartnerId = id });
        }

        private async Task<string> SaveImage(IFormFile imageFile, string folderName)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderName);
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

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            return $"{baseUrl}/{folderName}/{uniqueFileName}";
        }
        private void DeleteImage(string imageUrl, string folderName)
        {
            var fileName = Path.GetFileName(imageUrl);
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, folderName, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
