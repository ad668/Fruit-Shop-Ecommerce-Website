namespace OnlineFruitShop.Core.Interfaces
{
    public interface IEmailService
    {
        bool IsConfigured { get; }
        Task SendEmailAsync(string to, string subject, string htmlBody);
        Task SendEmailWithAttachmentAsync(string to, string subject, string htmlBody, byte[] attachmentData, string attachmentFileName);
    }
}
