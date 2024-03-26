using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace APIx.Models;

[Index("Cpf", Name = "AK_User_Cpf_Index", IsUnique = true)]
public class User(string cpf, string name)
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string Cpf { get; set; } = cpf;

    public string Name { get; set; } = name;

    [JsonIgnore]
    public ICollection<PaymentProviderAccount> PaymentProviderAccounts { get; set; } = null!;
}