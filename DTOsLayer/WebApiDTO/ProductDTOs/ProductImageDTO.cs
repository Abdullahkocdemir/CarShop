using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.ProductDTOs
{
    public class ProductImageDTO
    {
        public int Id { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsMainImage { get; set; } = false;
        public int Order { get; set; } = 0;
        public bool ShouldDelete { get; set; } = false;
    }
}
