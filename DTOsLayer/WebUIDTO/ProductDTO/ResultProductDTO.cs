using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOsLayer.WebUIDTO.ProductDTO
{
    public class ResultProductDTO
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ColorId { get; set; }
        public string ColorName { get; set; } = string.Empty; // İlişkili veri
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty; // İlişkili veri
        public int ModelId { get; set; }
        public string ModelName { get; set; } = string.Empty; // İlişkili veri
        public string Kilometer { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal Price { get; set; }
        public string? EngineSize { get; set; }
        public string? FuelType { get; set; }
        public string? Transmission { get; set; }
        public int? Horsepower { get; set; }
        public string? DriveType { get; set; }
        public int? DoorCount { get; set; }
        public int? SeatCount { get; set; }
        public string? Condition { get; set; }
        public bool HasAirbag { get; set; }
        public bool HasABS { get; set; }
        public bool HasESP { get; set; }
        public bool HasAirConditioning { get; set; }
        public bool HasSunroof { get; set; }
        public bool HasLeatherSeats { get; set; }
        public bool HasNavigationSystem { get; set; }
        public bool HasParkingSensors { get; set; }
        public bool HasBackupCamera { get; set; }
        public bool HasCruiseControl { get; set; }
        public string? Description { get; set; }
        public string? Features { get; set; }
        public string? DamageHistory { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? SellerType { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsPopular { get; set; }
    }
}
