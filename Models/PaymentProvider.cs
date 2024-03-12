namespace APIx.Models;

public class PaymentProvider(string token, string name)
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string Token { get; set; } = token;

    public string Name { get; set; } = name;

    public ICollection<PaymentProviderAccount> PaymentProviderAccounts { get; set; } = null!;
}