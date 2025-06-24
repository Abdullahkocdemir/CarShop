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
using System.Text;

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
            try
            {
                System.Diagnostics.Debug.WriteLine($"CreateProduct çağrıldı. ImageFiles null mu? {dto.ImageFiles == null}");

                if (dto.ImageFiles != null)
                {
                    System.Diagnostics.Debug.WriteLine($"ImageFiles sayısı: {dto.ImageFiles.Count()}");
                    foreach (var file in dto.ImageFiles)
                    {
                        if (file != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"Dosya: {file.FileName}, Boyut: {file.Length}");
                        }
                    }
                }

                var product = _mapper.Map<Product>(dto);
                product.Images = new List<ProductImage>();

                if (dto.ImageFiles != null && dto.ImageFiles.Any())
                {
                    var validFiles = dto.ImageFiles.Where(f => f != null && f.Length > 0).ToList();
                    System.Diagnostics.Debug.WriteLine($"İşlenecek geçerli dosya sayısı: {validFiles.Count}");

                    foreach (var imageFile in validFiles)
                    {
                        try
                        {
                            var fileName = await SaveImage(imageFile);
                            if (!string.IsNullOrEmpty(fileName))
                            {
                                product.Images.Add(new ProductImage
                                {
                                    ImageUrl = fileName,
                                    IsMainImage = product.Images.Count == 0, 
                                    Order = product.Images.Count
                                });
                                System.Diagnostics.Debug.WriteLine($"Resim kaydedildi: {fileName}");
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"Resim kaydedilemedi: {imageFile.FileName}");
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Resim kaydetme hatası: {ex.Message}");
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Hiç resim dosyası gönderilmedi");
                }

                System.Diagnostics.Debug.WriteLine($"Toplam kaydedilen resim sayısı: {product.Images.Count}");

                _productService.BAdd(product);
                PublishEntityCreated(product);

                return Ok(new
                {
                    Message = "Ürün başarıyla eklendi ve mesaj yayınlandı.",
                    ProductId = product.ProductId,
                    ImageCount = product.Images.Count
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CreateProduct genel hatası: {ex.Message}");
                return StatusCode(500, new { Message = "Ürün oluşturulurken bir hata oluştu.", Error = ex.Message });
            }
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
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                {
                    System.Diagnostics.Debug.WriteLine("SaveImage: Dosya null veya boş");
                    return string.Empty;
                }

                System.Diagnostics.Debug.WriteLine($"SaveImage: {imageFile.FileName} kaydediliyor, boyut: {imageFile.Length}");

                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "products");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                    System.Diagnostics.Debug.WriteLine($"Klasör oluşturuldu: {uploadsFolder}");
                }

                string fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

                if (!allowedExtensions.Contains(fileExtension))
                {
                    System.Diagnostics.Debug.WriteLine($"Geçersiz dosya uzantısı: {fileExtension}");
                    return string.Empty;
                }

                string datePart = DateTime.Now.ToString("ddMMyyyy");
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                var random = new Random();
                var randomPart = new string(Enumerable.Repeat(chars, 8) // 8 karakter random
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                string uniqueFileName = $"{datePart}-{randomPart}{fileExtension}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                System.Diagnostics.Debug.WriteLine($"Dosya başarıyla kaydedildi: {uniqueFileName}");
                return uniqueFileName;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveImage hatası: {ex.Message}");
                return string.Empty;
            }
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