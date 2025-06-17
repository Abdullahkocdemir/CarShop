using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.BroadcastDTO;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BroadcastsController : BaseEntityController
    {
        private readonly IBroadcastService _broadcastService;
        private readonly IMapper _mapper;
        protected override string EntityTypeName => "Broadcast";

        public BroadcastsController(IBroadcastService broadcastService, IMapper mapper, EnhancedRabbitMQService rabbitMqService)
            : base(rabbitMqService)
        {
            _broadcastService = broadcastService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetListAllBroadcasts()
        {
            var values = _mapper.Map<List<ResultBroadcastDTO>>(_broadcastService.BGetListAll());
            return Ok(values);
        }

        [HttpGet("{id}")]
        public IActionResult GetBroadcastById(int id)
        {
            var value = _broadcastService.BGetById(id);
            return Ok(value);
        }

        [HttpPost]
        public IActionResult Create(CreateBroadcastDTO dto)
        {
            var broadcast = _mapper.Map<Broadcast>(dto);
            _broadcastService.BAdd(broadcast);
            PublishEntityCreated(broadcast);
            return Ok(new { Message = "Yayın başarıyla eklendi ve mesaj gönderildi.", BroadcastId = broadcast.BroadcastId });
        }

        [HttpPut]
        public IActionResult UpdateBroadcast(UpdateBroadcastDTO dto)
        {
            var existingBroadcast = _broadcastService.BGetById(dto.BroadcastId);
            _mapper.Map(dto, existingBroadcast);
            _broadcastService.BUpdate(existingBroadcast);
            PublishEntityUpdated(existingBroadcast);

            return Ok(new { Message = "Yayın başarıyla güncellendi ve mesaj yayınlandı.", BroadcastId = existingBroadcast.BroadcastId });
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteBroadcast(int id)
        {
            var broadcastToDelete = _broadcastService.BGetById(id);
            _broadcastService.BDelete(broadcastToDelete);
            PublishEntityDeleted(broadcastToDelete);

            return Ok(new { Message = "Yayın başarıyla silindi ve mesaj yayınlandı.", BroadcastId = id });
        }
    }
}