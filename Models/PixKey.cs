using Microsoft.EntityFrameworkCore;

namespace APIx.Models;

[Index("Value", Name = "AK_PixKey_Value", IsUnique = true)]
public class PixKey(string type, string value)
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string Type { get; set; } = type;

    public string Value { get; set; } = value;

    public int PaymentProviderAccountId { get; set; }

    public PaymentProviderAccount PaymentProviderAccount { get; set; } = null!;

    public ICollection<Payment> Payments { get; set; } = null!;
}