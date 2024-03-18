namespace APIx.Models;

public class User(string cpf, string name)
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string Cpf { get; set; } = cpf;

    public string Name { get; set; } = name;

    public ICollection<PaymentProviderAccount> PaymentProviderAccounts { get; set; } = null!;
}