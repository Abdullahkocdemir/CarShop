using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.WhyUseDTO;
using DTOsLayer.WebApiDTO.WhyUseReasonDTO; 
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhyUsesController : BaseEntityController
    {
        private readonly IWhyUseService _whyUseService;
        private readonly IWhyUseReasonService _whyUseReasonService;
        private readonly IMapper _mapper;
        protected override string EntityTypeName => "WhyUse";

        public WhyUsesController(
            IWhyUseService whyUseService,
            IWhyUseReasonService whyUseReasonService,
            IMapper mapper,
            EnhancedRabbitMQService rabbitMqService)
            : base(rabbitMqService)
        {
            _whyUseService = whyUseService;
            _whyUseReasonService = whyUseReasonService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetListAllWhyUses()
        {
            var whyUses = _whyUseService.BGetListAllWithReasons();
            var values = _mapper.Map<List<ResultWhyUseDTO>>(whyUses);
            return Ok(values);
        }

        [HttpGet("{id}")]
        public IActionResult GetWhyUseById(int id)
        {
            var value = _whyUseService.BGetByIdWithReasons(id);
            if (value == null)
            {
                return NotFound($"ID'si {id} olan 'Neden Biz?' kaydı bulunamadı.");
            }
            var resultDto = _mapper.Map<ResultWhyUseDTO>(value);
            return Ok(resultDto);
        }

        [HttpPost]
        public IActionResult CreateWhyUse(CreateWhyUseDTO dto)
        {
            var whyUse = _mapper.Map<WhyUse>(dto);
            _whyUseService.BAdd(whyUse);

            if (dto.WhyUseReasons != null && dto.WhyUseReasons.Any())
            {
                foreach (var reasonDto in dto.WhyUseReasons)
                {
                    var reason = _mapper.Map<WhyUseReason>(reasonDto);
                    reason.WhyUseId = whyUse.WhyUseId; 
                    _whyUseReasonService.BAdd(reason);
                }
            }

            PublishEntityCreated(whyUse);

            return CreatedAtAction(nameof(GetWhyUseById), new { id = whyUse.WhyUseId }, new { Message = "'Neden Biz?' kaydı başarıyla eklendi ve mesaj gönderildi.", WhyUseId = whyUse.WhyUseId });
        }


        [HttpPut]
        public IActionResult UpdateWhyUse(UpdateWhyUseDTO dto)
        {
            var existingWhyUse = _whyUseService.BGetByIdWithReasons(dto.WhyUseId);
            if (existingWhyUse == null)
            {
                return NotFound($"ID'si {dto.WhyUseId} olan 'Neden Biz?' kaydı bulunamadı.");
            }

            _mapper.Map(dto, existingWhyUse);
            if (existingWhyUse.WhyUseReasons != null)
            {
                foreach (var existingReason in existingWhyUse.WhyUseReasons.ToList()) 
                {
                    _whyUseReasonService.BDelete(existingReason);
                }
            }

            existingWhyUse.WhyUseReasons = new List<WhyUseReason>(); 
            if (dto.WhyUseReasons != null && dto.WhyUseReasons.Any())
            {
                foreach (var reasonDto in dto.WhyUseReasons)
                {
                    var newReason = _mapper.Map<WhyUseReason>(reasonDto);
                    newReason.WhyUseId = existingWhyUse.WhyUseId; 
                    existingWhyUse.WhyUseReasons.Add(newReason); 
                }
            }

            _whyUseService.BUpdate(existingWhyUse); 
            PublishEntityUpdated(existingWhyUse);

            return Ok(new { Message = "'Neden Biz?' kaydı başarıyla güncellendi ve mesaj yayınlandı.", WhyUseId = existingWhyUse.WhyUseId });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteWhyUse(int id)
        {
            var whyUseToDelete = _whyUseService.BGetByIdWithReasons(id);
            if (whyUseToDelete == null)
            {
                return NotFound($"ID'si {id} olan 'Neden Biz?' kaydı bulunamadı.");
            }
            _whyUseService.BDelete(whyUseToDelete); 
            PublishEntityDeleted(whyUseToDelete);

            return Ok(new { Message = "'Neden Biz?' kaydı başarıyla silindi ve mesaj yayınlandı.", WhyUseId = id });
        }
    }
}