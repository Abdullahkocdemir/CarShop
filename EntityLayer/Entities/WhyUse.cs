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
        public string MainTitle { get; set; } = string.Empty;       
        public string MainDescription { get; set; } = string.Empty; 
        public string VideoUrl { get; set; } = string.Empty;
        public virtual ICollection<WhyUseItem> Items { get; set; } = new List<WhyUseItem>();
    }
}
