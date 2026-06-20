using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OnlineFruitShop.API.Models;
using OnlineFruitShop.Core.Entities;
using OnlineFruitShop.Core.Interfaces;
using OnlineFruitShop.Infrastructure.Data;
using OnlineFruitShop.Infrastructure.Services;

namespace OnlineFruitShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<Fruit> _fruitRepo;
        private readonly IRepository<Invoice> _invoiceRepo;
        private readonly IRepository<User> _userRepo;
        private readonly IAuthService _authService;
        private readonly IPaymentService _paymentService;
        private readonly IEmailService _emailService;
        private readonly EmailSettings _emailSettings;
        private readonly ApplicationDbContext _context;
        private readonly IDeliveryReceiptService _deliveryReceiptService;

        public OrdersController(
            IRepository<Order> orderRepo,
            IRepository<Fruit> fruitRepo,
            IRepository<Invoice> invoiceRepo,
            IRepository<User> userRepo,
            IAuthService authService,
            IPaymentService paymentService,
            IEmailService emailService,
            IOptions<EmailSettings> emailOptions,
            ApplicationDbContext context,
            IDeliveryReceiptService deliveryReceiptService)
        {
            _orderRepo = orderRepo;
            _fruitRepo = fruitRepo;
            _invoiceRepo = invoiceRepo;
            _userRepo = userRepo;
            _authService = authService;
            _paymentService = paymentService;
            _emailService = emailService;
            _emailSettings = emailOptions.Value;
            _context = context;
            _deliveryReceiptService = deliveryReceiptService;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
        {
            if (request.Items == null || !request.Items.Any())
            {
                return BadRequest(new ApiResponse<string> { Success = false, Message = "No items in the order." });
            }

            decimal subtotal = 0m;
            var orderItemsTable = new StringBuilder();
            orderItemsTable.AppendLine("<table style='width:100%;border-collapse:collapse;margin-top:16px;'>");
            orderItemsTable.AppendLine("<thead><tr style='background:#f2f4f9;'><th style='padding:10px;border:1px solid #d8dbe8;text-align:left;'>Fruit</th><th style='padding:10px;border:1px solid #d8dbe8;text-align:right;'>Qty</th><th style='padding:10px;border:1px solid #d8dbe8;text-align:right;'>Unit Price</th><th style='padding:10px;border:1px solid #d8dbe8;text-align:right;'>Line Total</th></tr></thead>");
            orderItemsTable.AppendLine("<tbody>");

            var order = new Order
            {
                UserId = request.UserId,
                ShippingAddress = request.ShippingAddress,
                ShippingMethod = request.ShippingMethod,
                PaymentMethod = request.PaymentMethod,
                ShippingCharge = request.ShippingCharge,
                Tax = request.Tax,
                Status = request.PaymentMethod == "cod" ? "Confirmed (Cash on Delivery)" : "Pending Payment",
                TrackingNumber = $"TRK-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}".Substring(0, 50),
                TrackingStatus = request.PaymentMethod == "cod" ? "Awaiting pickup" : "Awaiting payment"
            };

            foreach (var item in request.Items)
            {
                var fruit = await _fruitRepo.GetByIdAsync(item.FruitId);
                if (fruit == null)
                    return BadRequest(new ApiResponse<string> { Success = false, Message = "Item not available." });

                if (fruit.Stock < item.Quantity)
                    return BadRequest(new ApiResponse<string> { Success = false, Message = $"Not enough stock for {fruit.Name}." });

                var unitPrice = fruit.DiscountPrice > 0 ? fruit.DiscountPrice : fruit.Price;
                order.Items.Add(new OrderItem
                {
                    FruitId = fruit.Id,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice
                });
                subtotal += unitPrice * item.Quantity;
                var quantityText = item.Quantity.ToString("0.##");
                var unitLabel = string.IsNullOrWhiteSpace(fruit.PriceUnit) ? string.Empty : $" {fruit.PriceUnit}";
                orderItemsTable.AppendLine($"<tr><td style='padding:10px;border:1px solid #d8dbe8;font-weight:600;'>{fruit.Name}</td><td style='padding:10px;border:1px solid #d8dbe8;text-align:right;'>{quantityText}{unitLabel}</td><td style='padding:10px;border:1px solid #d8dbe8;text-align:right;'>₹{unitPrice:N2}</td><td style='padding:10px;border:1px solid #d8dbe8;text-align:right;'>₹{unitPrice * item.Quantity:N2}</td></tr>");
                fruit.Stock = Math.Max(0m, fruit.Stock - item.Quantity);
                _fruitRepo.Update(fruit);
            }

            order.Subtotal = subtotal;
            order.Total = subtotal + order.Tax + order.ShippingCharge;

            await _orderRepo.AddAsync(order);
            await _orderRepo.SaveChangesAsync();

            if (request.PaymentMethod == "cod")
            {
                var invoice = new Invoice
                {
                    OrderId = order.Id,
                    InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}-{order.Id}",
                    Subtotal = order.Subtotal,
                    Tax = order.Tax,
                    ShippingCharge = order.ShippingCharge,
                    Total = order.Total,
                    CustomerEmail = request.ShippingEmail,
                    IssuedAt = DateTime.UtcNow
                };

                orderItemsTable.AppendLine($"</tbody><tfoot><tr><td colspan='3' style='padding:10px;border:1px solid #d8dbe8;text-align:right;font-weight:700;'>Subtotal</td><td style='padding:10px;border:1px solid #d8dbe8;text-align:right;font-weight:700;'>₹{subtotal:N2}</td></tr><tr><td colspan='3' style='padding:10px;border:1px solid #d8dbe8;text-align:right;font-weight:700;'>Tax</td><td style='padding:10px;border:1px solid #d8dbe8;text-align:right;'>₹{order.Tax:N2}</td></tr><tr><td colspan='3' style='padding:10px;border:1px solid #d8dbe8;text-align:right;font-weight:700;'>Shipping</td><td style='padding:10px;border:1px solid #d8dbe8;text-align:right;'>₹{order.ShippingCharge:N2}</td></tr><tr><td colspan='3' style='padding:10px;border:1px solid #d8dbe8;text-align:right;font-size:1.05rem;font-weight:800;'>Total</td><td style='padding:10px;border:1px solid #d8dbe8;text-align:right;font-size:1.05rem;font-weight:800;'>₹{order.Total:N2}</td></tr></tfoot></table>");
                var orderItemsHtml = orderItemsTable.ToString();

                await _invoiceRepo.AddAsync(invoice);
                await _invoiceRepo.SaveChangesAsync();

                var userBody = $@"
                    <h2>Order confirmed</h2>
                    <p>Your order <strong>#{order.Id}</strong> has been placed successfully.</p>
                    <p><strong>Payment method:</strong> Cash on Delivery</p>
                    <p><strong>Shipping method:</strong> {order.ShippingMethod}</p>
                    <p><strong>Tracking number:</strong> {order.TrackingNumber}</p>
                    <p><strong>Tracking status:</strong> {order.TrackingStatus}</p>
                    <p><strong>Shipping address:</strong> {order.ShippingAddress}</p>
                    {orderItemsHtml}
                    <p><strong>Total:</strong> ₹{order.Total:N2}</p>
                    <p>Your invoice number is <strong>{invoice.InvoiceNumber}</strong>.</p>
                    <p>We will notify you again when your order ships.</p>
                ";

                var adminBody = $@"
                    <h2>New COD order received</h2>
                    <p>Order <strong>#{order.Id}</strong> has been placed with Cash on Delivery.</p>
                    <p><strong>User email:</strong> {request.ShippingEmail}</p>
                    <p><strong>Shipping address:</strong> {order.ShippingAddress}</p>
                    <p><strong>Shipping method:</strong> {order.ShippingMethod}</p>
                    <p><strong>Total:</strong> ₹{order.Total:N2}</p>
                    <p><strong>Tracking number:</strong> {order.TrackingNumber}</p>
                ";

                var emailWarning = string.Empty;
                if (_emailService.IsConfigured && !string.IsNullOrEmpty(request.ShippingEmail) && !string.IsNullOrEmpty(_emailSettings.AdminEmail))
                {
                    try
                    {
                        await _emailService.SendEmailAsync(request.ShippingEmail, $"Your FruitShop order #{order.Id} is confirmed", userBody);
                        await _emailService.SendEmailAsync(_emailSettings.AdminEmail, $"New COD order #{order.Id} received", adminBody);
                    }
                    catch (Exception ex)
                    {
                        emailWarning = " Email notification failed. " + ex.Message;
                    }
                }
                else if (!_emailService.IsConfigured)
                {
                    emailWarning = " Email notifications are not configured.";
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Cash on Delivery order placed. Invoice has been created." + emailWarning,
                    Data = new { order.Id, order.Total, order.TrackingNumber, order.TrackingStatus }
                });
            }

            var paymentId = await _paymentService.CreatePaymentIntentAsync(order.Total, request.Currency);
            return Ok(new ApiResponse<object> { Success = true, Message = "Order created. Proceed to payment.", Data = new { order.Id, order.Total, paymentId } });
        }

        [HttpPost("payment")]
        public async Task<IActionResult> ConfirmPayment([FromBody] PaymentRequest request)
        {
            var paymentId = await _paymentService.CreatePaymentIntentAsync(request.Amount, request.Currency);
            var confirmed = await _paymentService.ConfirmPaymentAsync(paymentId);
            if (!confirmed)
                return BadRequest(new ApiResponse<string> { Success = false, Message = "Payment failed." });

            return Ok(new ApiResponse<object> { Success = true, Message = "Payment confirmed.", Data = new { paymentId } });
        }

        [HttpGet("track/{orderId:int}")]
        public async Task<IActionResult> TrackOrder(int orderId)
        {
            var order = (await _orderRepo.FindAsync(o => o.Id == orderId)).FirstOrDefault();
            if (order == null)
                return NotFound(new ApiResponse<string> { Success = false, Message = "Order not found." });

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Order tracking information retrieved.",
                Data = new
                {
                    order.Id,
                    order.Status,
                    order.ShippingMethod,
                    order.PaymentMethod,
                    order.TrackingNumber,
                    order.TrackingStatus,
                    order.Total,
                    order.CreatedAt
                }
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .ToListAsync();

            var data = orders.Select(order => new
            {
                order.Id,
                order.UserId,
                CustomerEmail = order.User?.Email ?? string.Empty,
                order.ShippingAddress,
                order.ShippingMethod,
                order.PaymentMethod,
                order.TrackingNumber,
                order.TrackingStatus,
                order.Status,
                order.Subtotal,
                order.Tax,
                order.ShippingCharge,
                order.Total,
                order.CreatedAt
            }).OrderByDescending(o => o.CreatedAt);

            return Ok(new ApiResponse<object> { Success = true, Message = "Orders retrieved.", Data = data });
        }

        private string GenerateOTP()
        {
            var random = new Random();
            return random.Next(1000, 9999).ToString();
        }

        [HttpPost("confirm/{orderId:int}")]
        public async Task<IActionResult> ConfirmOrder(int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null)
                return NotFound(new ApiResponse<string> { Success = false, Message = "Order not found." });

            if (order.Status.Equals("Out for Delivery", StringComparison.OrdinalIgnoreCase) ||
                order.TrackingStatus.Equals("Out for Delivery", StringComparison.OrdinalIgnoreCase))
            {
                return Ok(new ApiResponse<string> { Success = true, Message = "Order is already out for delivery." });
            }

            order.Status = "Out for Delivery";
            order.TrackingStatus = "Out for Delivery";
            order.DeliveryOTP = GenerateOTP();
            _orderRepo.Update(order);
            await _orderRepo.SaveChangesAsync();

            var customer = await _userRepo.GetByIdAsync(order.UserId);
            var invoice = (await _invoiceRepo.FindAsync(i => i.OrderId == order.Id)).FirstOrDefault();
            var customerEmail = customer?.Email ?? invoice?.CustomerEmail;

            if (!string.IsNullOrEmpty(customerEmail) && _emailService.IsConfigured)
            {
                var body = $@"
                    <h2>Your order is out for delivery</h2>
                    <p>Your order <strong>#{order.Id}</strong> is now out for delivery.</p>
                    <p><strong>Tracking number:</strong> {order.TrackingNumber}</p>
                    <p><strong>Shipping method:</strong> {order.ShippingMethod}</p>
                    <p><strong>Total paid:</strong> ₹{order.Total:N2}</p>
                    <p><strong style='color:#2563eb;font-size:1.2rem;'>Delivery OTP: {order.DeliveryOTP}</strong></p>
                    <p style='color:#666;font-size:0.9rem;'>This OTP is valid until your order is delivered.</p>
                    <p>Thank you for shopping with FruitShop.</p>
                ";

                try
                {
                    await _emailService.SendEmailAsync(customerEmail, $"Your FruitShop order #{order.Id} is out for delivery", body);
                }
                catch (Exception ex)
                {
                    return Ok(new ApiResponse<string> { Success = true, Message = $"Order status updated, but notification failed: {ex.Message}" });
                }
            }

            return Ok(new ApiResponse<string> { Success = true, Message = "Order confirmed as out for delivery and customer notified." });
        }

        [HttpPost("verify-delivery-otp/{orderId:int}")]
        public async Task<IActionResult> VerifyDeliveryOTP(int orderId, [FromBody] OTPVerificationRequest request)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .ThenInclude(i => i.Fruit)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return NotFound(new ApiResponse<string> { Success = false, Message = "Order not found." });

            if (string.IsNullOrEmpty(request.Otp))
            {
                return BadRequest(new ApiResponse<string> { Success = false, Message = "OTP is required." });
            }

            if (!order.Status.Equals("Out for Delivery", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new ApiResponse<string> { Success = false, Message = "Order is not out for delivery." });
            }

            if (string.IsNullOrEmpty(order.DeliveryOTP))
            {
                return BadRequest(new ApiResponse<string> { Success = false, Message = "OTP not generated for this order." });
            }

            if (!order.DeliveryOTP.Equals(request.Otp, StringComparison.Ordinal))
            {
                return BadRequest(new ApiResponse<string> { Success = false, Message = "Invalid OTP. Please try again." });
            }

            order.Status = "Delivered";
            order.TrackingStatus = "Delivered";
            order.DeliveryOTP = null;
            _orderRepo.Update(order);
            await _orderRepo.SaveChangesAsync();

            var customer = await _userRepo.GetByIdAsync(order.UserId);
            var invoice = (await _invoiceRepo.FindAsync(i => i.OrderId == order.Id)).FirstOrDefault();
            var customerEmail = customer?.Email ?? invoice?.CustomerEmail;

            if (!string.IsNullOrEmpty(customerEmail) && _emailService.IsConfigured)
            {
                var body = $@"
                    <h2>Your order has been delivered</h2>
                    <p>Your order <strong>#{order.Id}</strong> has been successfully delivered.</p>
                    <p><strong>Tracking number:</strong> {order.TrackingNumber}</p>
                    <p><strong>Delivered on:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}</p>
                    <p>Please find the delivery receipt in the attached PDF.</p>
                    <p>Thank you for shopping with FruitShop.</p>
                ";

                try
                {
                    // Generate delivery receipt PDF
                    var customerName = customer?.Name ?? "Customer";
                    var pdfData = _deliveryReceiptService.GenerateDeliveryReceipt(order, customerName);
                    
                    // Send email with PDF attachment
                    await _emailService.SendEmailWithAttachmentAsync(
                        customerEmail,
                        $"Your FruitShop order #{order.Id} has been delivered",
                        body,
                        pdfData,
                        $"FruitShop_Order_{order.Id}_Receipt.pdf"
                    );
                }
                catch (Exception ex)
                {
                    return Ok(new ApiResponse<string> { Success = true, Message = $"Order marked as delivered, but email/PDF failed: {ex.Message}" });
                }
            }

            return Ok(new ApiResponse<string> { Success = true, Message = "Order marked as delivered successfully. Customer notified with delivery receipt." });
        }

        [HttpGet("delivery-orders")]
        public async Task<IActionResult> GetDeliveryOrders()
        {
            var orders = await _context.Orders
                .Where(o => o.Status == "Out for Delivery")
                .Include(o => o.User)
                .Include(o => o.Items)
                .ThenInclude(i => i.Fruit)
                .ToListAsync();

            var data = orders.Select(order => new
            {
                order.Id,
                order.UserId,
                CustomerName = order.User?.Name ?? string.Empty,
                CustomerEmail = order.User?.Email ?? string.Empty,
                order.ShippingAddress,
                order.ShippingMethod,
                order.PaymentMethod,
                order.TrackingNumber,
                order.TrackingStatus,
                order.Status,
                order.Subtotal,
                order.Tax,
                order.ShippingCharge,
                order.Total,
                order.CreatedAt,
                Items = order.Items.Select(item => new
                {
                    item.FruitId,
                    item.Quantity,
                    item.UnitPrice,
                    FruitName = item.Fruit?.Name ?? ""
                }).ToList()
            }).OrderByDescending(o => o.CreatedAt);

            return Ok(new ApiResponse<object> { Success = true, Message = "Delivery orders retrieved.", Data = data });
        }
    }
}
