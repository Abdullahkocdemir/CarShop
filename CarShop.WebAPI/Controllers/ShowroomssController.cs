using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.ShowroomDTO;
using EntityLayer.Entities; 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowroomssController : BaseEntityController 
    {
        private readonly IShowroomService _showroomsService; 
        private readonly IMapper _mapper;
        protected override string EntityTypeName => "Showroom"; 

        public ShowroomssController(IShowroomService showroomsService, IMapper mapper, EnhancedRabbitMQService rabbitMqService)
            : base(rabbitMqService)
        {
            _showroomsService = showroomsService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetListAllShowroomss()
        {
            var values = _mapper.Map<List<ResultShowroomDTO>>(_showroomsService.BGetListAll());
            return Ok(values);
        }

        [HttpGet("{id}")]
        public IActionResult GetShowroomsById(int id)
        {
            var value = _showroomsService.BGetById(id);
            return Ok(value);
        }

        [HttpPost]
        public IActionResult CreateShowrooms(CreateShowroomDTO dto)
        {
            var showrooms = _mapper.Map<Showroom>(dto);
            _showroomsService.BAdd(showrooms);
            PublishEntityCreated(showrooms);

            return Ok(new { Message = "Showroom başarıyla eklendi ve mesaj gönderildi.", ShowroomsId = showrooms.ShowroomId });
        }

        [HttpPut]
        public IActionResult UpdateShowrooms(UpdateShowroomDTO dto)
        {
            var existingShowrooms = _showroomsService.BGetById(dto.ShowroomId);
            _mapper.Map(dto, existingShowrooms);
            _showroomsService.BUpdate(existingShowrooms);
            PublishEntityUpdated(existingShowrooms);

            return Ok(new { Message = "Showroom başarıyla güncellendi ve mesaj yayınlandı.", ShowroomsId = existingShowrooms.ShowroomId });
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteShowrooms(int id)
        {
            var showroomsToDelete = _showroomsService.BGetById(id);
            _showroomsService.BDelete(showroomsToDelete);
            PublishEntityDeleted(showroomsToDelete);

            return Ok(new { Message = "Showroom başarıyla silindi ve mesaj yayınlandı.", ShowroomsId = id });
        }
    }
}