using OnlineFruitShop.Core.Entities;

namespace OnlineFruitShop.Core.Interfaces
{
    public interface IAuthService
    {
        Task<string> GenerateTokenAsync(User user);
        Task<bool> VerifyPasswordAsync(string hashedPassword, string password);
        Task<string> HashPasswordAsync(string password);
    }
}
