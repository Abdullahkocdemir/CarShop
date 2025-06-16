using AutoMapper; // AutoMapper namespace'ini ekleyin
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
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
        private readonly IMapper _mapper; // IMapper'ı tanımlayın
        protected override string EntityTypeName => "Product";

        public ProductController(IProductService productService, EnhancedRabbitMQService rabbitMqService, IMapper mapper)
            : base(rabbitMqService)
        {
            _productService = productService;
            _mapper = mapper; // IMapper'ı enjekte edin
        }

        [HttpPost]
        public IActionResult AddProduct(CreateProductDTO dto)
        {
            try
            {
                // AutoMapper kullanarak DTO'dan Varlığa eşleme yapın
                var product = _mapper.Map<Product>(dto);
                // Oluşturma tarihini, eğer mapping profile'da özel bir mantıkla ele alınmadıysa, açıkça ayarlayabilirsiniz
                // product.CreatedDate = DateTime.UtcNow;

                _productService.BAdd(product);
                PublishEntityCreated(product);

                return Ok(new { Message = "Ürün başarıyla eklendi ve mesaj yayınlandı.", ProductId = product.ProductId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddProduct'ta hata: {ex.Message}");
                return StatusCode(500, "Ürün eklenirken bir hata oluştu.");
            }
        }

        [HttpPut]
        public IActionResult UpdateProduct(UpdateProductDTO dto)
        {
            try
            {
                var existingProduct = _productService.BGetById(dto.ProductId);
                if (existingProduct == null)
                    return NotFound("Ürün bulunamadı.");

                _mapper.Map(dto, existingProduct);
                _productService.BUpdate(existingProduct);
                PublishEntityUpdated(existingProduct);

                return Ok(new { Message = "Ürün başarıyla güncellendi ve mesaj yayınlandı.", ProductId = existingProduct.ProductId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateProduct'ta hata: {ex.Message}");
                return StatusCode(500, "Ürün güncellenirken bir hata oluştu.");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                var product = _productService.BGetById(id);
                if (product == null)
                    return NotFound("Ürün bulunamadı.");

                _productService.BDelete(product);
                PublishEntityDeleted(product);

                return Ok(new { Message = "Ürün başarıyla silindi ve mesaj yayınlandı.", ProductId = id });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteProduct'ta hata: {ex.Message}");
                return StatusCode(500, "Ürün silinirken bir hata oluştu.");
            }
        }
    }
}