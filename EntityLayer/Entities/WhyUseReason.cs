using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
    // Her bir "Neden Bizi Seçmelisiniz" maddesi (check-mark listesindeki her bir öğe)
    public class WhyUseReason
    {
        public int WhyUseReasonId { get; set; }
        public string ReasonText { get; set; } = string.Empty;  // Örneğin: "Lorem ipsum dolor sit amet, consectetur adipiscing elit."
        public int DisplayOrder { get; set; }                   // Maddelerin HTML listesinde hangi sırada görüneceğini belirler (1, 2, 3...)
        public string IconCssClass { get; set; } = "fa fa-check-circle"; // Örneğin: "fa fa-check-circle" gibi ikon sınıfı

        // Bu neden maddesinin hangi ana WhyUse bölümüne ait olduğunu gösteren yabancı anahtar.
        public int WhyUseId { get; set; }
        public WhyUse WhyUse { get; set; } = null!; // İlişkisel özellik (Navigation property)
    }
}
