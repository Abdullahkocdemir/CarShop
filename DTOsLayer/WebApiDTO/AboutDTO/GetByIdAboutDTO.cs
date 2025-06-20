using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.AboutDTO
{
    public class GetByIdAboutDTO
    {
        public int AboutId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SmallTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
