namespace OnlineFruitShop.Core.Interfaces
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentIntentAsync(decimal amount, string currency);
        Task<bool> ConfirmPaymentAsync(string paymentId);
    }
}
