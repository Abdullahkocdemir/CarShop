using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    // Neden Bizi Seçmelisiniz bölümünün genel bilgileri (başlık, açıklama, video vb.)
    public class WhyUse
    {
        public int WhyUseId { get; set; }
        public string MainTitle { get; set; } = string.Empty;       // Örneğin: "Neden İnsanlar Bizi Seçiyor?"
        public string MainDescription { get; set; } = string.Empty; // Örneğin: "Duis aute irure dolorin reprehenderits volupta..."
        public string VideoUrl { get; set; } = string.Empty;        // Videonun oynatılacağı URL

        // Bir WhyUse bölümüne ait birden fazla neden maddesi olabilir.
        // Bu, EF Core ile ilişkisel bir tablo bağlantısı kurar.
        public ICollection<WhyUseReason>? WhyUseReasons { get; set; }
    }
}
