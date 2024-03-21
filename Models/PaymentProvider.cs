using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace APIx.Models;

[Index("Token", Name = "IX_PaymentProviderAccount_Token", IsUnique = true)]
public class PaymentProvider(string token, string name)
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string Token { get; set; } = token;
    public string Name { get; set; } = name;
    public string PostPaymentUrl { get; set; } = "/payments/pix";
    public string PatchPaymentUrl { get; set; } = "/payments/pix";
    public string PostConcilliationUrl { get; set; } = "/concilliation/pix";

    [JsonIgnore]
    public ICollection<PaymentProviderAccount> PaymentProviderAccounts { get; set; } = null!;

    [JsonIgnore]
    public ICollection<Concilliation> Concilliations { get; set; } = null!;
}