using System.ComponentModel.DataAnnotations;

namespace OnlineFruitShop.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        [Required, MaxLength(128)]
        public string Name { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Customer";
        public bool IsActive { get; set; } = true;
    }
}
