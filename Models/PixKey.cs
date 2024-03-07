using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIx.Models;

public class PixKey(string type, string value)
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string Type { get; set; } = type;

    public string Value { get; set; } = value;

    public int PaymentProviderAccountId { get; set; }

    public PaymentProviderAccount PaymentProviderAccount { get; set; } = null!;
}