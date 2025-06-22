using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 
using System.Collections.Generic;

namespace EntityLayer.Entities
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        // Temel Bilgiler
        [Required]
        public int ColorId { get; set; }
        [ForeignKey(nameof(ColorId))]
        public virtual Color? Color { get; set; }

        [Required]
        public int BrandId { get; set; }
        [ForeignKey(nameof(BrandId))]
        public virtual Brand? Brand { get; set; }

        [Required]
        public int ModelId { get; set; }
        [ForeignKey(nameof(ModelId))]
        public virtual Model? Model { get; set; }

        [StringLength(20)]
        public string Kilometer { get; set; } = string.Empty;

        [Range(1900, 2030)]
        public int Year { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        // Teknik Özellikler
        [StringLength(50)]
        public string? EngineSize { get; set; } // Motor Hacmi (örn: "1.6L", "2.0L")

        [StringLength(20)]
        public string? FuelType { get; set; } // Yakıt Türü (Benzin, Dizel, Hibrit, Elektrik)

        [StringLength(20)]
        public string? Transmission { get; set; } // Vites (Manuel, Otomatik, Yarı Otomatik)

        public int? Horsepower { get; set; } // Beygir Gücü

        [StringLength(20)]
        public string? DriveType { get; set; } // Çekiş (Önden, Arkadan, 4x4)

        public int? DoorCount { get; set; } // Kapı Sayısı

        public int? SeatCount { get; set; } // Koltuk Sayısı

        // Araç Durumu
        [StringLength(20)]
        public string? Condition { get; set; } // Durumu (Sıfır, İkinci El, Hasarlı)

        // Güvenlik ve Donanım
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

        // Açıklamalar
        [StringLength(2000)]
        public string? Description { get; set; } // Genel açıklama

        [StringLength(500)]
        public string? Features { get; set; } // Öne çıkan özellikler

        [StringLength(500)]
        public string? DamageHistory { get; set; } // Hasar geçmişi (varsa)

        // Konum Bilgileri
        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(50)]
        public string? District { get; set; }

        // Satıcı Bilgileri
        [StringLength(20)]
        public string? SellerType { get; set; } // Galeri, Sahibinden, vs.

        // Görsel ve Medya
        public virtual ICollection<ProductImage>? Images { get; set; }

        // Durum ve Onay
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsPopular { get; set; } = false;

    }

    public class ProductImage
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public virtual Product? Product { get; set; }

        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMainImage { get; set; } = false;
        public int Order { get; set; } = 0;
    }
}