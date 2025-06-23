using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.FeatureSubstancesDTO
{
    public class GetByIdFeatureSubstancesDTO
    {
        public int FeatureSubstanceId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
