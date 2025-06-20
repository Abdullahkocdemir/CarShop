using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.AboutFeature
{
    public class UpdateAboutFeatureDTO
    {
        public int AboutFeatureId { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string ExistingImageUrl { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
