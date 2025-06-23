namespace CarShop.WebUI.Models
{
    public class ProductListViewModel
    {
        public List<DTOsLayer.WebUIDTO.ProductDTO.ResultProductDTO> Products { get; set; }
        public List<DTOsLayer.WebUIDTO.BrandDTO.ResultBrandDTO> Brands { get; set; }
        public List<DTOsLayer.WebUIDTO.ModelsDTO.ResultModelDTO> Models { get; set; }
        public List<DTOsLayer.WebUIDTO.ColorDTO.ResultColorDTO> Colors { get; set; }
        public List<string> TransmissionTypes { get; set; }
        public List<string> EngineSizes { get; set; }
        public List<string> Conditions { get; set; }
        public List<string> FuelTypes { get; set; }
        public List<string> DriveTypes { get; set; }

        public ProductListViewModel()
        {
            Products = new List<DTOsLayer.WebUIDTO.ProductDTO.ResultProductDTO>();
            Brands = new List<DTOsLayer.WebUIDTO.BrandDTO.ResultBrandDTO>();
            Models = new List<DTOsLayer.WebUIDTO.ModelsDTO.ResultModelDTO>();
            Colors = new List<DTOsLayer.WebUIDTO.ColorDTO.ResultColorDTO>();

            TransmissionTypes = new List<string> { "Manuel", "Otomatik", "Yarı Otomatik" };
            EngineSizes = new List<string> { "1.0L", "1.2L", "1.4L", "1.6L", "1.8L", "2.0L", "2.5L", "3.0L", "Diğer" }; 
            Conditions = new List<string> { "Sıfır", "İkinci El", "Hasarlı" };
            FuelTypes = new List<string> { "Benzin", "Dizel", "Hibrit", "Elektrik", "LPG" };
            DriveTypes = new List<string> { "Önden", "Arkadan", "4x4" };
        }
    }
}
