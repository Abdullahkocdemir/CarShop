using AutoMapper; // AutoMapper için using
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.BannerDTO;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannersController : BaseEntityController
    {
        private readonly IBannerService _bannerService;
        private readonly IMapper _mapper;
        protected override string EntityTypeName => "Banner";

        public BannersController(IBannerService bannerService, EnhancedRabbitMQService rabbitMqService, IMapper mapper)
            : base(rabbitMqService)
        {
            _bannerService = bannerService;
            _mapper = mapper;
        }


        [HttpGet]
        public IActionResult GetListAllBanner()
        {
            var values = _mapper.Map<List<ResultBannerDTO>>(_bannerService.BGetListAll());
            return Ok(values);
        }


        [HttpPost]
        public IActionResult CreateBanner(CreateBannerDTO dto)
        {
            var banner = _mapper.Map<Banner>(dto);
            _bannerService.BAdd(banner);
            PublishEntityCreated(banner);
            return Ok(new { Message = "Banner added and message published.", BannerId = banner.BannerId });
        }

        [HttpPut]
        public IActionResult UpdateBanner(UpdateBannerDTO dto)
        {
            var existingBanner = _bannerService.BGetById(dto.BannerId);
             _mapper.Map(dto, existingBanner);
            _bannerService.BUpdate(existingBanner);
            PublishEntityUpdated(existingBanner);

            return Ok(new { Message = "Afiş Güncellendi ve Mesaj Yayınlandı.", BannerId = existingBanner.BannerId });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBanner(int id)
        {
            var banner = _bannerService.BGetById(id);
            _bannerService.BDelete(banner);
            PublishEntityDeleted(banner);
            return Ok(new { Message = "Afiş Silindi Ve Yayınlandı", BannerId = id });
        }

        [HttpGet("{id}")]
        public IActionResult GetByIdBanner(int id)
        {
            var value = _bannerService.BGetById(id);
            return Ok(value);
        }


    }
}