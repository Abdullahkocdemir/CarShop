using AutoMapper;
using BusinessLayer.Abstract; 
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.CallBackTitleDTO; 
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pipelines.Sockets.Unofficial;
using SharpCompress.Common;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Collections.Generic;
using System;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallBackTitlesController : BaseEntityController
    {
        private readonly ICallBackTitleService _callBackTitleService;
        private readonly IMapper _mapper;
        protected override string EntityTypeName => "CallBackTitle";

        public CallBackTitlesController(ICallBackTitleService callBackTitleService, EnhancedRabbitMQService rabbitMqService, IMapper mapper)
            : base(rabbitMqService)
        {
            _callBackTitleService = callBackTitleService;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult GetListCallBackTitle()
        {
            var callBackTitles = _callBackTitleService.BGetListAll();
            var callBackTitleDtos = _mapper.Map<List<ResultCallBackTitleDTO>>(callBackTitles);
            return Ok(callBackTitleDtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetByIdCallBackTitle(int id)
        {
            var callBackTitle = _callBackTitleService.BGetById(id);
            if (callBackTitle == null)
            {
                return NotFound($"Geri arama başlığı ID {id} ile bulunamadı.");
            }
            var callBackTitleDto = _mapper.Map<GetByIdCallBackTitleDTO>(callBackTitle);
            return Ok(callBackTitleDto);
        }

        [HttpPost]
        public IActionResult CreateCallBackTitle(CreateCallBackTitleDTO dto)
        {
            var callBackTitle = _mapper.Map<CallBackTitle>(dto);
            _callBackTitleService.BAdd(callBackTitle);
            PublishEntityCreated(callBackTitle);

            return Ok(new { Message = "Geri arama başlığı başarıyla eklendi ve mesaj yayınlandı.", CallBackTitleId = callBackTitle.CallBackTitleId });
        }

        [HttpPut]
        public IActionResult UpdateCallBackTitle(UpdateCallBackTitleDTO dto)
        {
            var existingCallBackTitle = _callBackTitleService.BGetById(dto.CallBackTitleId);
            if (existingCallBackTitle == null)
            {
                return NotFound($"Geri arama başlığı ID {dto.CallBackTitleId} ile güncelleme için bulunamadı.");
            }
            _mapper.Map(dto, existingCallBackTitle);
            _callBackTitleService.BUpdate(existingCallBackTitle);
            PublishEntityUpdated(existingCallBackTitle);

            return Ok(new { Message = "Geri arama başlığı başarıyla güncellendi ve mesaj yayınlandı.", CallBackTitleId = existingCallBackTitle.CallBackTitleId });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCallBackTitle(int id)
        {
            var callBackTitle = _callBackTitleService.BGetById(id);
            if (callBackTitle == null)
            {
                return NotFound($"Geri arama başlığı ID {id} ile silme için bulunamadı.");
            }
            _callBackTitleService.BDelete(callBackTitle);
            PublishEntityDeleted(callBackTitle);

            return Ok(new { Message = "Geri arama başlığı başarıyla silindi ve mesaj yayınlandı.", CallBackTitleId = id });
        }
    }
}