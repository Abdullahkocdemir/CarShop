using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebApiDTO.NewLatestDTO
{
    public class ResultNewLatestDTO
    {
        public int NewLatestId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}
