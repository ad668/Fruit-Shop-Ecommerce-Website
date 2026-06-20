using System.ComponentModel.DataAnnotations;

namespace OnlineFruitShop.Core.Entities
{
    public class Invoice
    {
        public int Id { get; set; }
        [Required]
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        [Required, MaxLength(100)]
        public string InvoiceNumber { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal ShippingCharge { get; set; }
        public decimal Total { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    }
}
