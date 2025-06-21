using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.PartnerDTO
{
    public  class GetByIdPartnerDTO
    {
        public int PartnerId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
