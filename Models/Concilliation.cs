namespace APIx.Models;

public class Concilliation(string fileUrl, int paymentProviderId)
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string FileUrl { get; set; } = fileUrl;
    public string Status { get; set; } = "PENDING";
    public string? OutputFileUrl { get; set; }
    public int PaymentProviderId { get; set; } = paymentProviderId;
    public PaymentProvider PaymentProvider { get; set; } = null!;
}