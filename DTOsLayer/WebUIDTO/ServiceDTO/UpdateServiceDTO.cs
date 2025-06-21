using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.ServiceDTO
{
    public class UpdateServiceDTO
    {
        public int ServiceId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile? ImageFile { get; set; } 
        public string? ExistingImageUrl { get; set; } 
    }
}
