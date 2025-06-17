using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.BrandDTO;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : BaseEntityController
    {
        private readonly IBrandService _brandService;
        private readonly IMapper _mapper;
        protected override string EntityTypeName => "Brand";

        public BrandsController(IBrandService brandService, EnhancedRabbitMQService rabbitMqService, IMapper mapper)
            : base(rabbitMqService)
        {
            _brandService = brandService;
            _mapper = mapper;
        }


        [HttpGet]
        public IActionResult GetListBrand()
        {
            var brands = _brandService.BGetListAll();
            var brandDtos = _mapper.Map<List<ResultBrandDTO>>(brands);
            return Ok(brandDtos);
        }
        [HttpGet("{id}")]
        public IActionResult GetByIdBrand(int id)
        {
            var brand = _brandService.BGetById(id);
            var brandDto = _mapper.Map<GetByIdBrandDTO>(brand);
            return Ok(brandDto);
        }
        [HttpPost]
        public IActionResult CreateBrand(CreateBrandDTO dto)
        {
            var brand = _mapper.Map<Brand>(dto);
            _brandService.BAdd(brand);
            PublishEntityCreated(brand);

            return Ok(new { Message = "Marka başarıyla eklendi ve mesaj yayınlandı.", BrandId = brand.BrandId });
        }
        [HttpPut]
        public IActionResult UpdateBrand(UpdateBrandDTO dto)
        {
            var existingBrand = _brandService.BGetById(dto.BrandId);
            _mapper.Map(dto, existingBrand);
            _brandService.BUpdate(existingBrand);
            PublishEntityUpdated(existingBrand);

            return Ok(new { Message = "Marka başarıyla güncellendi ve mesaj yayınlandı.", BrandId = existingBrand.BrandId });
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteBrand(int id)
        {
            var brand = _brandService.BGetById(id);
            _brandService.BDelete(brand);
            PublishEntityDeleted(brand);
            return Ok(new { Message = "Marka başarıyla silindi ve mesaj yayınlandı.", BrandId = id });
        }


    }
}