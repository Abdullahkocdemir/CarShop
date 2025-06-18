using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.FeatureImageDTO
{
    public class ResultFeatureImageDTO
    {
        public int FeatureImageId { get; set; }
        public int FeatureId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }
}
