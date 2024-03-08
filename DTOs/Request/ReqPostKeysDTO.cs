using System.ComponentModel.DataAnnotations;
using APIx.Models;

namespace APIx.RequestDTOs;
public class ReqPostKeysDTO
{
    [Required]
    public required KeyDTO Key { get; set; }

    [Required]
    public required UserDTO User { get; set; }

    [Required]
    public required AccountDTO Account { get; set; }

    public string GetUserCpf()
    {
        return User.Cpf;
    }

    public PixKey GetPixKey()
    {
        return new PixKey(Key.Type, Key.Value);
    }

    public PaymentProviderAccount GetPaymentProviderAccount()
    {
        return new PaymentProviderAccount(Account.Number, Account.Agency);
    }
}

public enum KeyType
{
    CPF,
    Email,
    Phone,
    Random
}

public class KeyDTO
{
    [Required]
    [EnumDataType(typeof(KeyType), ErrorMessage = "Invalid key type")]
    public required string Type { get; set; }
    public required string Value { get; set; }
}

public class UserDTO
{
    [Required]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF must have 11 number digits")]
    public required string Cpf { get; set; }
}

public class AccountDTO
{
    [Required]
    [RegularExpression(@"^\d+$", ErrorMessage = "Account number must be a number")]
    public required string Number { get; set; }

    [Required]
    [RegularExpression(@"^\d+$", ErrorMessage = "Agency number must be a number")]
    public required string Agency { get; set; }
}