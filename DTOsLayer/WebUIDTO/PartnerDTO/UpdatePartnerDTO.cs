using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.PartnerDTO
{
    public class UpdatePartnerDTO
    {
        public int PartnerId { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? ExistingImageUrl { get; set; }
    }
}
