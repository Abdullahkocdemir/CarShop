using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.ServiceDTO
{
    public class UpdateServiceDTO
    {
        public int ServiceId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile? ImageFile { get; set; } // Use IFormFile for upload
        public string? ExistingImageUrl { get; set; } // To retain existing image if no new file is uploaded
    }
}
