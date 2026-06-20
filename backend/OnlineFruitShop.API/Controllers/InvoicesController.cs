using Microsoft.AspNetCore.Mvc;
using OnlineFruitShop.API.Models;
using OnlineFruitShop.Core.Entities;
using OnlineFruitShop.Core.Interfaces;

namespace OnlineFruitShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly IRepository<Invoice> _invoiceRepo;
        private readonly IRepository<Order> _orderRepo;

        public InvoicesController(IRepository<Invoice> invoiceRepo, IRepository<Order> orderRepo)
        {
            _invoiceRepo = invoiceRepo;
            _orderRepo = orderRepo;
        }

        [HttpGet("{orderId:int}")]
        public async Task<IActionResult> GetByOrder(int orderId)
        {
            var invoice = (await _invoiceRepo.FindAsync(i => i.OrderId == orderId)).FirstOrDefault();
            if (invoice == null)
                return NotFound(new ApiResponse<string> { Success = false, Message = "Invoice not found." });

            return Ok(new ApiResponse<Invoice> { Success = true, Message = "Invoice retrieved.", Data = invoice });
        }

        [HttpPost("generate")]
        public async Task<IActionResult> Generate([FromBody] Order order)
        {
            var invoice = new Invoice
            {
                OrderId = order.Id,
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}-{order.Id}",
                Subtotal = order.Subtotal,
                Tax = order.Tax,
                ShippingCharge = order.ShippingCharge,
                Total = order.Total,
                CustomerEmail = order.User?.Email ?? string.Empty,
                IssuedAt = DateTime.UtcNow
            };

            await _invoiceRepo.AddAsync(invoice);
            await _invoiceRepo.SaveChangesAsync();
            return Ok(new ApiResponse<Invoice> { Success = true, Message = "Invoice generated.", Data = invoice });
        }
    }
}
