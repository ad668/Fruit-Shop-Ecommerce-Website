using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineFruitShop.API.Models;
using OnlineFruitShop.Core.Interfaces;

namespace OnlineFruitShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public ContactController(IEmailService emailService, IConfiguration configuration)
        {
            _emailService = emailService;
            _configuration = configuration;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendContactMessage([FromBody] ContactMessageRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string> 
                { 
                    Success = false, 
                    Message = "Invalid request data." 
                });
            }

            try
            {
                var adminEmail = _configuration["EmailSettings:AdminEmail"] ?? "admin@fruitly.com";

                // Email to customer
                var customerEmailBody = $@"
                    <h2>Thank you for contacting Fruitly!</h2>
                    <p>Dear {request.Name},</p>
                    <p>We have received your message and will get back to you soon.</p>
                    <hr>
                    <h3>Your Message Details:</h3>
                    <p><strong>Subject:</strong> {request.Subject}</p>
                    <p><strong>Message:</strong> {request.Message}</p>
                    <p><strong>Phone:</strong> {request.Phone}</p>
                    <hr>
                    <p>Best regards,<br>Fruitly Team</p>
                ";

                // Email to admin
                var adminEmailBody = $@"
                    <h2>New Contact Message from Customer</h2>
                    <p><strong>Name:</strong> {request.Name}</p>
                    <p><strong>Email:</strong> {request.Email}</p>
                    <p><strong>Phone:</strong> {request.Phone}</p>
                    <p><strong>Subject:</strong> {request.Subject}</p>
                    <hr>
                    <h3>Message:</h3>
                    <p>{request.Message}</p>
                    <hr>
                    <p>Received at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>
                ";

                // Send email to customer
                await _emailService.SendEmailAsync(
                    request.Email ?? "",
                    "Thank you for contacting Fruitly - Message Received",
                    customerEmailBody
                );

                // Send email to admin
                await _emailService.SendEmailAsync(
                    adminEmail,
                    $"New Contact Message from {request.Name}",
                    adminEmailBody
                );

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    Message = "Your message has been sent successfully. We will contact you soon.",
                    Data = null
                });
            }
            catch
            {
                return StatusCode(500, new ApiResponse<string>
                {
                    Success = false,
                    Message = "An error occurred while sending your message. Please try again later."
                });
            }
        }
    }

    public class ContactMessageRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }
    }
}
