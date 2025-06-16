using AutoMapper;
using BusinessLayer.Abstract; // IWhyUseService için
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.WhyUseDTO; // WhyUse DTO'larınız
using EntityLayer.Entities; // WhyUse entity'niz
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhyUsesController : BaseEntityController
    {
        private readonly IWhyUseService _whyUseService; 
        private readonly IMapper _mapper;
        protected override string EntityTypeName => "WhyUse"; 

        public WhyUsesController(IWhyUseService whyUseService, IMapper mapper, EnhancedRabbitMQService rabbitMqService)
            : base(rabbitMqService)
        {
            _whyUseService = whyUseService;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult GetListAllWhyUses() 
        {
            var values = _mapper.Map<List<ResultWhyUseDTO>>(_whyUseService.BGetListAll());
            return Ok(values);
        }

        [HttpGet("{id}")]
        public IActionResult GetWhyUseById(int id) 
        {
            var value = _whyUseService.BGetById(id);
            if (value == null)
            {
                return NotFound($"ID'si {id} olan 'Neden Biz?' kaydı bulunamadı.");
            }
            return Ok(value);
        }

        [HttpPost]
        public IActionResult CreateWhyUse(CreateWhyUseDTO dto) 
        {
            var whyUse = _mapper.Map<WhyUse>(dto);
            _whyUseService.BAdd(whyUse);
            PublishEntityCreated(whyUse);

            return Ok(new { Message = "'Neden Biz?' kaydı başarıyla eklendi ve mesaj gönderildi.", WhyUseId = whyUse.WhyUseId });
        }


        [HttpPut]
        public IActionResult UpdateWhyUse(UpdateWhyUseDTO dto) 
        {
            var existingWhyUse = _whyUseService.BGetById(dto.WhyUseId);
            _mapper.Map(dto, existingWhyUse);
            _whyUseService.BUpdate(existingWhyUse);
            PublishEntityUpdated(existingWhyUse);

            return Ok(new { Message = "'Neden Biz?' kaydı başarıyla güncellendi ve mesaj yayınlandı.", WhyUseId = existingWhyUse.WhyUseId });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteWhyUse(int id) 
        {
            var whyUseToDelete = _whyUseService.BGetById(id);
            _whyUseService.BDelete(whyUseToDelete);
            PublishEntityDeleted(whyUseToDelete);

            return Ok(new { Message = "'Neden Biz?' kaydı başarıyla silindi ve mesaj yayınlandı.", WhyUseId = id });
        }
    }
}