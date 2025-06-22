using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.ProductDTOs;
using EntityLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting; 
using Microsoft.AspNetCore.Http; 
using System.Collections.Generic;
using System.IO; 
using System.Linq;
using System.Threading.Tasks;
using System;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : BaseEntityController 
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        protected override string EntityTypeName => "Product";

        public ProductsController(IProductService productService, EnhancedRabbitMQService rabbitMqService, IMapper mapper, IWebHostEnvironment webHostEnvironment)
            : base(rabbitMqService)
        {
            _productService = productService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public IActionResult GetListAllProducts()
        {
            var values = _productService.BGetProductsWithDetails(); 
            var result = values.Select(product =>
            {
                var dto = _mapper.Map<ResultProductDTO>(product);
                dto.ImageUrls = product.Images?.Select(img => $"{Request.Scheme}://{Request.Host}/products/{img.ImageUrl}").ToList() ?? new List<string>();
                return dto;
            }).ToList();
            return Ok(result);
        }
        [HttpPost]
        [Consumes("multipart/form-data")] 
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDTO dto)
        {
            var product = _mapper.Map<Product>(dto);
            product.Images = new List<ProductImage>();

            foreach (var imageFile in dto.ImageFiles)
            {
                var fileName = await SaveImage(imageFile);
                if (!string.IsNullOrEmpty(fileName))
                {
                    product.Images.Add(new ProductImage
                    {
                        ImageUrl = fileName,
                        IsMainImage = false, 
                        Order = product.Images.Count 
                    });
                }
            }

            _productService.BAdd(product);
            PublishEntityCreated(product); 
            return Ok(new { Message = "Ürün başarıyla eklendi ve mesaj yayınlandı.", ProductId = product.ProductId });
        }
        [HttpPut]
        [Consumes("multipart/form-data")] 
        public async Task<IActionResult> UpdateProduct([FromForm] UpdateProductDTO dto)
        {
            var existingProduct = _productService.BGetProductByIdWithDetails(dto.ProductId); 

            if (existingProduct == null)
            {
                return NotFound($"Ürün ID {dto.ProductId} ile bulunamadı.");
            }

            _mapper.Map(dto, existingProduct);
            existingProduct.UpdatedDate = DateTime.UtcNow; 

            var currentImages = existingProduct.Images?.ToDictionary(x => x.Id) ?? new Dictionary<int, ProductImage>();
            var imagesToKeepOrUpdate = new List<ProductImage>();
            var imageFilesToProcess = new List<(ProductImageDTO dto, IFormFile? file)>();

            foreach (var imageDto in dto.Images)
            {
                if (imageDto.ShouldDelete && imageDto.Id != 0)
                {
                    if (currentImages.TryGetValue(imageDto.Id, out var imgToDelete))
                    {
                        DeleteImage(imgToDelete.ImageUrl);
                    }
                }
                else if (imageDto.ImageFile != null) 
                {
                    imageFilesToProcess.Add((imageDto, imageDto.ImageFile));
                }
                else if (imageDto.Id != 0 && currentImages.TryGetValue(imageDto.Id, out var existingImage)) 
                {
                    existingImage.IsMainImage = imageDto.IsMainImage;
                    existingImage.Order = imageDto.Order;
                    imagesToKeepOrUpdate.Add(existingImage);
                }
            }

            foreach (var (imageDto, imageFile) in imageFilesToProcess)
            {
                string fileName = await SaveImage(imageFile!); 
                if (!string.IsNullOrEmpty(fileName))
                {
                    if (imageDto.Id != 0 && currentImages.TryGetValue(imageDto.Id, out var existingImage)) 
                    {
                        DeleteImage(existingImage.ImageUrl); 
                        existingImage.ImageUrl = fileName;
                        existingImage.IsMainImage = imageDto.IsMainImage;
                        existingImage.Order = imageDto.Order;
                        imagesToKeepOrUpdate.Add(existingImage);
                    }
                    else 
                    {
                        imagesToKeepOrUpdate.Add(new ProductImage
                        {
                            ProductId = existingProduct.ProductId,
                            ImageUrl = fileName,
                            IsMainImage = imageDto.IsMainImage,
                            Order = imageDto.Order
                        });
                    }
                }
            }
            existingProduct.Images?.Clear();
            foreach (var img in imagesToKeepOrUpdate)
            {
                existingProduct.Images?.Add(img);
            }


            _productService.BUpdate(existingProduct);
            PublishEntityUpdated(existingProduct); 

            return Ok(new { Message = "Ürün başarıyla güncellendi ve mesaj yayınlandı.", ProductId = existingProduct.ProductId });
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _productService.BGetProductByIdWithDetails(id); 
            if (product == null)
            {
                return NotFound($"Ürün ID {id} ile bulunamadı.");
            }

            if (product.Images != null)
            {
                foreach (var image in product.Images)
                {
                    DeleteImage(image.ImageUrl);
                }
            }

            _productService.BDelete(product);
            PublishEntityDeleted(product); 
            return Ok(new { Message = "Ürün başarıyla silindi ve mesaj yayınlandı.", ProductId = id });
        }
        [HttpGet("{id}")]
        public IActionResult GetByIdProduct(int id)
        {
            var value = _productService.BGetProductByIdWithDetails(id); 
            if (value == null)
            {
                return NotFound($"Ürün ID {id} ile bulunamadı.");
            }

            var result = _mapper.Map<GetByIdProductDTO>(value);
            result.ImageUrls = value.Images?.Select(img => $"{Request.Scheme}://{Request.Host}/products/{img.ImageUrl}").ToList() ?? new List<string>();

            return Ok(result);
        }
        [HttpGet("WithBrand")]
        public IActionResult GetProductsWithBrand()
        {
            var values = _productService.BGetProductWithBrand();
            var result = values.Select(product => new ResultProductDTO 
            {
                ProductId = product.ProductId,
                Name = product.Name,
                BrandName = product.Brand?.BrandName ?? string.Empty, 
            }).ToList();
            return Ok(result);
        }
        private async Task<string> SaveImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return string.Empty;
            }
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "products");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            return uniqueFileName;
        }

        private void DeleteImage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "products", fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}