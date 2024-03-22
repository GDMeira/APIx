using APIx.RequestDTOs;

namespace APIx.Models;

public class Concilliation(string fileUrl, string postback, DateOnly date, int paymentProviderId)
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string FileUrl { get; set; } = fileUrl;
    public string Status { get; set; } = "PENDING";
    public string Postback { get; set; } = postback;
    public DateOnly Date { get; set; } = date;
    public int PaymentProviderId { get; set; } = paymentProviderId;
    public PaymentProvider PaymentProvider { get; set; } = null!;
}