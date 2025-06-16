using AutoMapper;
using BusinessLayer.Abstract; // INewLatestService için
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.NewLatestDTO; // Yeni DTO'larınız
using EntityLayer.Entities; // NewLatest entity'niz
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewLatestsController : BaseEntityController
    {
        private readonly INewLatestService _newLatestService; 
        private readonly IMapper _mapper;
        protected override string EntityTypeName => "NewLatest"; 

        public NewLatestsController(INewLatestService newLatestService, IMapper mapper, EnhancedRabbitMQService rabbitMqService)
            : base(rabbitMqService)
        {
            _newLatestService = newLatestService;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult GetListAllNewLatests() 
        {
            var values = _mapper.Map<List<ResultNewLatestDTO>>(_newLatestService.BGetListAll());
            return Ok(values);
        }

        [HttpGet("{id}")]
        public IActionResult GetNewLatestById(int id) 
        {
            var value = _newLatestService.BGetById(id);
            if (value == null)
            {
                return NotFound($"ID'si {id} olan 'En Yeni' kaydı bulunamadı.");
            }
            return Ok(value);
        }

        [HttpPost]
        public IActionResult CreateNewLatest(CreateNewLatestDTO dto) 
        {

            var newLatest = _mapper.Map<NewLatest>(dto);
            _newLatestService.BAdd(newLatest);
            PublishEntityCreated(newLatest);

            return Ok(new { Message = "'En Yeni' kaydı başarıyla eklendi ve mesaj gönderildi.", NewLatestId = newLatest.NewLatestId });
        }

        [HttpPut]
        public IActionResult UpdateNewLatest(UpdateNewLatestDTO dto) 
        {
            var existingNewLatest = _newLatestService.BGetById(dto.NewLatestId);

            _mapper.Map(dto, existingNewLatest);
            _newLatestService.BUpdate(existingNewLatest);
            PublishEntityUpdated(existingNewLatest);

            return Ok(new { Message = "'En Yeni' kaydı başarıyla güncellendi ve mesaj yayınlandı.", NewLatestId = existingNewLatest.NewLatestId });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteNewLatest(int id) 
        {
            var newLatestToDelete = _newLatestService.BGetById(id);

            _newLatestService.BDelete(newLatestToDelete);
            PublishEntityDeleted(newLatestToDelete);

            return Ok(new { Message = "'En Yeni' kaydı başarıyla silindi ve mesaj yayınlandı.", NewLatestId = id });
        }
    }
}