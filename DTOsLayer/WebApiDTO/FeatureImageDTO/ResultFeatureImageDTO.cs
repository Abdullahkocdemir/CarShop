using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.FeatureImageDTO
{
    public class ResultFeatureImageDTO
    {
        public int FeatureImageId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
