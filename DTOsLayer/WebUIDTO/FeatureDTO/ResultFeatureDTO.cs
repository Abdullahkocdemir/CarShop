using DTOsLayer.WebApiDTO.FeatureImageDTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.FeatureDTO
{
    public class ResultFeatureDTO
    {
        public int FeatureId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SmallTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        // Özelliğe ait resimlerin listesi.
        public List<ResultFeatureImageDTO>? FeatureImages { get; set; }
    }
}
