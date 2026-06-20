namespace OnlineFruitShop.API.Models
{
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterDeliveryPartnerRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterAdminRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class OTPVerificationRequest
    {
        public string Otp { get; set; } = string.Empty;
    }

    public class CheckoutRequest
    {
        public int UserId { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string ShippingEmail { get; set; } = string.Empty;
        public string ShippingMethod { get; set; } = "Standard";
        public string PaymentMethod { get; set; } = "card";
        public decimal Tax { get; set; }
        public decimal ShippingCharge { get; set; }
        public string Currency { get; set; } = "INR";
        public List<OrderItemRequest> Items { get; set; } = new();
    }

    public class OrderItemRequest
    {
        public int FruitId { get; set; }
        public decimal Quantity { get; set; }
    }

    public class PaymentRequest
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
    }
}
