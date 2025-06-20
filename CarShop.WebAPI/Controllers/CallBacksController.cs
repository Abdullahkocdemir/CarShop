using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.CallBackDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pipelines.Sockets.Unofficial;
using SharpCompress.Common;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Collections.Generic;
using System;
using EntityLayer.Entities;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallBacksController : BaseEntityController
    {
        private readonly ICallBackService _callBackService;
        private readonly IMapper _mapper;
        protected override string EntityTypeName => "CallBack";

        public CallBacksController(ICallBackService callBackService, EnhancedRabbitMQService rabbitMqService, IMapper mapper)
            : base(rabbitMqService)
        {
            _callBackService = callBackService;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult GetListCallBack()
        {
            var callBacks = _callBackService.BGetListAll();
            var callBackDtos = _mapper.Map<List<ResultCallBackDTO>>(callBacks);
            return Ok(callBackDtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetByIdCallBack(int id)
        {
            var callBack = _callBackService.BGetById(id);
            if (callBack == null)
            {
                return NotFound($"CallBack entry with ID {id} not found.");
            }
            var callBackDto = _mapper.Map<GetByIdCallBackDTO>(callBack);
            return Ok(callBackDto);
        }

        [HttpPost]
        public IActionResult CreateCallBack(CreateCallBackDTO dto)
        {
            var callBack = _mapper.Map<CallBack>(dto);
            _callBackService.BAdd(callBack);
            PublishEntityCreated(callBack);

            return Ok(new { Message = "Geri arama başarıyla eklendi ve mesaj yayınlandı.", CallBackId = callBack.CallBackId });
        }
        [HttpPut]
        public IActionResult UpdateCallBack(UpdateCallBackDTO dto)
        {
            var existingCallBack = _callBackService.BGetById(dto.CallBackId);
            if (existingCallBack == null)
            {
                return NotFound($"CallBack entry with ID {dto.CallBackId} not found for update.");
            }
            _mapper.Map(dto, existingCallBack);
            _callBackService.BUpdate(existingCallBack);
            PublishEntityUpdated(existingCallBack);

            return Ok(new { Message = "Geri arama başarıyla güncellendi ve mesaj yayınlandı.", CallBackId = existingCallBack.CallBackId });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCallBack(int id)
        {
            var callBack = _callBackService.BGetById(id);
            if (callBack == null)
            {
                return NotFound($"CallBack entry with ID {id} not found for deletion.");
            }
            _callBackService.BDelete(callBack);
            PublishEntityDeleted(callBack);

            return Ok(new { Message = "Geri arama başarıyla silindi ve mesaj yayınlandı.", CallBackId = id });
        }
    }
}