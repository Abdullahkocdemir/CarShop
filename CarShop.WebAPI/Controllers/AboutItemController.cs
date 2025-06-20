using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.AboutItem;
using DTOsLayer.WebApiDTO.AboutItemDTO;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AboutItemController : BaseEntityController
    {
        private readonly IAboutItemService _aboutItemService;
        private readonly IMapper _mapper;
        protected override string EntityTypeName => "AboutItem";

        public AboutItemController(IAboutItemService aboutItemService, EnhancedRabbitMQService rabbitMqService, IMapper mapper)
            : base(rabbitMqService)
        {
            _aboutItemService = aboutItemService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetListAboutItem()
        {
            var aboutItems = _aboutItemService.BGetListAll();
            var aboutItemDtos = _mapper.Map<List<ResultAboutItemDTO>>(aboutItems);
            return Ok(aboutItemDtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetByIdAboutItem(int id)
        {
            var aboutItem = _aboutItemService.BGetById(id);
            var aboutItemDto = _mapper.Map<GetByIdAboutItemDTO>(aboutItem);
            return Ok(aboutItemDto);
        }

        [HttpPost]
        public IActionResult CreateAboutItem(CreateAboutItemDTO dto)
        {
            var aboutItem = _mapper.Map<AboutItem>(dto);
            _aboutItemService.BAdd(aboutItem);
            PublishEntityCreated(aboutItem);

            return Ok(new { Message = "AboutItem başarıyla eklendi ve mesaj yayınlandı.", AboutItemId = aboutItem.AboutItemId });
        }

        [HttpPut]
        public IActionResult UpdateAboutItem(UpdateAboutItemDTO dto)
        {
            var existingAboutItem = _aboutItemService.BGetById(dto.AboutItemId);
            _mapper.Map(dto, existingAboutItem);
            _aboutItemService.BUpdate(existingAboutItem);
            PublishEntityUpdated(existingAboutItem);

            return Ok(new { Message = "AboutItem başarıyla güncellendi ve mesaj yayınlandı.", AboutItemId = existingAboutItem.AboutItemId });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAboutItem(int id)
        {
            var aboutItem = _aboutItemService.BGetById(id);
            _aboutItemService.BDelete(aboutItem);
            PublishEntityDeleted(aboutItem);
            return Ok(new { Message = "AboutItem başarıyla silindi ve mesaj yayınlandı.", AboutItemId = id });
        }
    }
}