using AutoMapper;
using BusinessLayer.Abstract; // IServiceService için
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.ServiceDTO; // Service DTO'larınız
using EntityLayer.Entities; // Service entity'niz
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : BaseEntityController
    {
        private readonly IServiceService _serviceService; 
        private readonly IMapper _mapper;
        protected override string EntityTypeName => "Service"; 

        public ServicesController(IServiceService serviceService, IMapper mapper, EnhancedRabbitMQService rabbitMqService)
            : base(rabbitMqService)
        {
            _serviceService = serviceService;
            _mapper = mapper;
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
        public IActionResult CreateService(CreateServiceDTO dto) 
        {
            var service = _mapper.Map<Service>(dto);
            _serviceService.BAdd(service);
            PublishEntityCreated(service);

            return Ok(new { Message = "Hizmet başarıyla eklendi ve mesaj gönderildi.", ServiceId = service.ServiceId });
        }
        [HttpPut]
        public IActionResult UpdateService(UpdateServiceDTO dto) 
        {
            var existingService = _serviceService.BGetById(dto.ServiceId);
            _mapper.Map(dto, existingService);
            _serviceService.BUpdate(existingService);
            PublishEntityUpdated(existingService);

            return Ok(new { Message = "Hizmet başarıyla güncellendi ve mesaj yayınlandı.", ServiceId = existingService.ServiceId });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteService(int id) 
        {
            var serviceToDelete = _serviceService.BGetById(id);
            _serviceService.BDelete(serviceToDelete);
            PublishEntityDeleted(serviceToDelete);

            return Ok(new { Message = "Hizmet başarıyla silindi ve mesaj yayınlandı.", ServiceId = id });
        }
    }
}