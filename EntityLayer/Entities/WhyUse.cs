using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class WhyUse
    {
        public int WhyUseId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string SmallTitle { get; set; } = string.Empty;
    }
}
