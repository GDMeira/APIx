namespace APIx.Models;

public class PaymentProviderAccount(string number, string agency)
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string Number { get; set; } = number;

    public string Agency { get; set; } = agency;

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public int PaymentProviderId { get; set; }

    public PaymentProvider PaymentProvider { get; set; } = null!;

    public ICollection<PixKey> PixKeys { get; set; } = null!;

    public ICollection<Payment> Payments { get; set; } = null!;
}