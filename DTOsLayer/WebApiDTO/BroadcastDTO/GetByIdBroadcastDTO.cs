using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.BroadcastDTO
{
    public class GetByIdBroadcastDTO
    {
        public int BroadcastId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SmallTitle { get; set; } = string.Empty;
    }
}
