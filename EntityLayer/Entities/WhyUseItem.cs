using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    public class WhyUseItem
    {
        public int Id { get; set; }

        public int WhyUseId { get; set; }

        [ForeignKey(nameof(WhyUseId))]
        public WhyUse? WhyUse { get; set; }

        public string Content { get; set; } = string.Empty; 
    }

}
