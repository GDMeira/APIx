using Microsoft.EntityFrameworkCore;

namespace APIx.Models;

[Index("CreatedAt", Name = "IX_Payment_CreatedAt", IsUnique = false)]
public class Payment(int amount, string? description)
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int Amount { get; set; } = amount;

    public string? Description { get; set; } = description;

    public string Status { get; set; } = "PENDING";

    public int PaymentProviderAccountId { get; set; }

    public PaymentProviderAccount PaymentProviderAccount { get; set; } = null!;

    public int PixKeyId { get; set; }

    public PixKey PixKey { get; set; } = null!;
}