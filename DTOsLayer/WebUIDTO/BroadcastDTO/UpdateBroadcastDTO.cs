using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.BroadcastDTO
{
    public class UpdateBroadcastDTO
    {
        public int BroadcastId { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string SmallTitle { get; set; } = string.Empty;
    }
}
