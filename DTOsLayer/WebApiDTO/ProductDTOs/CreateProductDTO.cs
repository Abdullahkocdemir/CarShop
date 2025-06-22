using Microsoft.AspNetCore.Http;

namespace DTOsLayer.WebApiDTO.ProductDTOs
{
    public class CreateProductDTO
    {
        public string Name { get; set; } = string.Empty;
        public int ColorId { get; set; }
        public int BrandId { get; set; }
        public int ModelId { get; set; }
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
        public bool HasAirbag { get; set; } = false;
        public bool HasABS { get; set; } = false;
        public bool HasESP { get; set; } = false;
        public bool HasAirConditioning { get; set; } = false;
        public bool HasSunroof { get; set; } = false;
        public bool HasLeatherSeats { get; set; } = false;
        public bool HasNavigationSystem { get; set; } = false;
        public bool HasParkingSensors { get; set; } = false;
        public bool HasBackupCamera { get; set; } = false;
        public bool HasCruiseControl { get; set; } = false;
        public string? Description { get; set; }
        public string? Features { get; set; }
        public string? DamageHistory { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? SellerType { get; set; }
        public List<IFormFile> ImageFiles { get; set; } = new List<IFormFile>();
    }
}
