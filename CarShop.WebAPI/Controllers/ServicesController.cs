using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.ServiceDTO;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting; 
using System.IO; 

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : BaseEntityController
    {
        private readonly IServiceService _serviceService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment; 
        protected override string EntityTypeName => "Service";

        public ServicesController(IServiceService serviceService, IMapper mapper, EnhancedRabbitMQService rabbitMqService, IWebHostEnvironment webHostEnvironment)
            : base(rabbitMqService)
        {
            _serviceService = serviceService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment; 
        }

        [HttpGet]
        public IActionResult GetListAllServices()
        {
            var values = _mapper.Map<List<ResultServiceDTO>>(_serviceService.BGetListAll());
            return Ok(values);
        }

        [HttpGet("{id}")]
        public IActionResult GetServiceById(int id)
        {
            var value = _serviceService.BGetById(id);
            if (value == null)
            {
                return NotFound($"ID'si {id} olan hizmet bulunamadı.");
            }
            return Ok(value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateService([FromForm] CreateServiceDTO dto) 
        {
            var service = _mapper.Map<Service>(dto);

            if (dto.ImageFile != null)
            {
                service.ImageUrl = await SaveImage(dto.ImageFile);
            }

            _serviceService.BAdd(service);
            PublishEntityCreated(service);

            return Ok(new { Message = "Hizmet başarıyla eklendi ve mesaj gönderildi.", ServiceId = service.ServiceId, ImageUrl = service.ImageUrl });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateService([FromForm] UpdateServiceDTO dto) 
        {
            var existingService = _serviceService.BGetById(dto.ServiceId);
            if (existingService == null)
            {
                return NotFound($"ID'si {dto.ServiceId} olan hizmet bulunamadı.");
            }

            _mapper.Map(dto, existingService);

            if (dto.ImageFile != null)
            {
                if (!string.IsNullOrEmpty(existingService.ImageUrl))
                {
                    DeleteImage(existingService.ImageUrl);
                }
                existingService.ImageUrl = await SaveImage(dto.ImageFile);
            }
            else if (!string.IsNullOrEmpty(dto.ExistingImageUrl))
            {
                existingService.ImageUrl = dto.ExistingImageUrl;
            }

            _serviceService.BUpdate(existingService);
            PublishEntityUpdated(existingService);

            return Ok(new { Message = "Hizmet başarıyla güncellendi ve mesaj yayınlandı.", ServiceId = existingService.ServiceId, ImageUrl = existingService.ImageUrl });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteService(int id)
        {
            var serviceToDelete = _serviceService.BGetById(id);
            if (serviceToDelete == null)
            {
                return NotFound($"ID'si {id} olan hizmet bulunamadı.");
            }

            if (!string.IsNullOrEmpty(serviceToDelete.ImageUrl))
            {
                DeleteImage(serviceToDelete.ImageUrl);
            }

            _serviceService.BDelete(serviceToDelete);
            PublishEntityDeleted(serviceToDelete);

            return Ok(new { Message = "Hizmet başarıyla silindi ve mesaj yayınlandı.", ServiceId = id });
        }

        private async Task<string> SaveImage(IFormFile imageFile)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "services");
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
            return $"{baseUrl}/services/{uniqueFileName}";
        }

        private void DeleteImage(string imageUrl)
        {
            var fileName = Path.GetFileName(imageUrl);
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "services", fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}