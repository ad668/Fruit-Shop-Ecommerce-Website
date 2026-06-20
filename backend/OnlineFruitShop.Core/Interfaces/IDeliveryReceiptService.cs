using OnlineFruitShop.Core.Entities;

namespace OnlineFruitShop.Core.Interfaces
{
    public interface IDeliveryReceiptService
    {
        byte[] GenerateDeliveryReceipt(Order order, string customerName);
    }
}
