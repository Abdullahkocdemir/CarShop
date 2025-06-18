using Microsoft.AspNetCore.Http; // IFormFile için bu using gereklidir
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.FeatureDTO
{
    public class CreateFeatureDTO
    {
        public string Title { get; set; } = string.Empty;
        public string SmallTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<IFormFile>? ImageFiles { get; set; }
    }
}