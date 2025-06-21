using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.CalltoActionDTO
{
    public class GetByIdCalltoActionDTO
    {
        public int CalltoActionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SmallTitle { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
