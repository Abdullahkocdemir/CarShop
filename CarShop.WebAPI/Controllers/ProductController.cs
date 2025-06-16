using AutoMapper; // AutoMapper namespace'ini ekleyin
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.BannerDTO;
using DTOsLayer.WebApiDTO.ProductDTOs;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : BaseEntityController
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        protected override string EntityTypeName => "Product";

        public ProductController(IProductService productService, EnhancedRabbitMQService rabbitMqService, IMapper mapper)
            : base(rabbitMqService)
        {
            _productService = productService;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult GetListAllProduct()
        {
            var values = _mapper.Map<List<ResultProductDTO>>(_productService.BGetListAll());
            return Ok(values);
        }
        [HttpPost]
        public IActionResult AddProduct(CreateProductDTO dto)
        {
            var product = _mapper.Map<Product>(dto);
            _productService.BAdd(product);
            PublishEntityCreated(product);
            return Ok(new { Message = "Ürün başarıyla eklendi ve mesaj yayınlandı.", ProductId = product.ProductId });
        }

        [HttpPut]
        public IActionResult UpdateProduct(UpdateProductDTO dto)
        {
            var existingProduct = _productService.BGetById(dto.ProductId);
            _mapper.Map(dto, existingProduct);
            _productService.BUpdate(existingProduct);
            PublishEntityUpdated(existingProduct);

            return Ok(new { Message = "Ürün başarıyla güncellendi ve mesaj yayınlandı.", ProductId = existingProduct.ProductId });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _productService.BGetById(id);
            _productService.BDelete(product);
            PublishEntityDeleted(product);
            return Ok(new { Message = "Ürün başarıyla silindi ve mesaj yayınlandı.", ProductId = id });
        }
    }
}