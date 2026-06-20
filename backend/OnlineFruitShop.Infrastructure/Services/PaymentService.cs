using OnlineFruitShop.Core.Interfaces;

namespace OnlineFruitShop.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        public Task<bool> ConfirmPaymentAsync(string paymentId)
        {
            // Placeholder for real gateway verification.
            return Task.FromResult(true);
        }

        public Task<string> CreatePaymentIntentAsync(decimal amount, string currency)
        {
            // Placeholder for payment gateway call; return a dummy payment ID.
            return Task.FromResult($"pi_{Guid.NewGuid():N}");
        }
    }
}
