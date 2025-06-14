public class CreateProductDTO
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int BrandId { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
