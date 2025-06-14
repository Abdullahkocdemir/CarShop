using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class Broadcast
    {
        public int BroadcastId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SmallTitle { get; set; } = string.Empty;
    }
}
