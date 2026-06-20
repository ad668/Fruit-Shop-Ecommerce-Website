namespace OnlineFruitShop.Infrastructure.Services
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 25;
        public bool UseSsl { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string AdminEmail { get; set; } = string.Empty;

        public bool IsConfigured => !string.IsNullOrWhiteSpace(SmtpHost) && SmtpPort > 0 && !string.IsNullOrWhiteSpace(FromEmail);
    }
}
