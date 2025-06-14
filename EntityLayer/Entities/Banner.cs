using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class Banner
    {
        public int BannerId { get; set; }
        public string SmallTitle { get; set; } = string.Empty;
        public string SubTitle { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;

    }
}
