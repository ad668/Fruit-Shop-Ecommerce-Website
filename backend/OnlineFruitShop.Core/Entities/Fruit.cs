using System.ComponentModel.DataAnnotations;

namespace OnlineFruitShop.Core.Entities
{
    public class Fruit
    {
        public int Id { get; set; }
        [Required, MaxLength(120)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; }
        public decimal Stock { get; set; }
        public string Category { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string ImageData { get; set; } = string.Empty;
        public bool IsFeatured { get; set; }
        public string Status { get; set; } = "Available";
        public string PriceUnit { get; set; } = "KG";
    }
}
