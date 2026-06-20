using System.ComponentModel.DataAnnotations;

namespace OnlineFruitShop.Core.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        [Required, MaxLength(256)]
        public string ShippingAddress { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string ShippingMethod { get; set; } = "Standard";
        [Required, MaxLength(50)]
        public string PaymentMethod { get; set; } = "card";
        [Required, MaxLength(100)]
        public string TrackingNumber { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string TrackingStatus { get; set; } = "Order confirmed";
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal ShippingCharge { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? DeliveryOTP { get; set; }
        public DateTime? OTPExpiryTime { get; set; }
        public List<OrderItem> Items { get; set; } = new();
    }
}
