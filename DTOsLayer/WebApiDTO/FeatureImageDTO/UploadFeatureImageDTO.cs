using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.FeatureImageDTO
{
    public class UploadFeatureImageDTO
    {
        public IFormFile? ImageFile { get; set; }
    }
}
