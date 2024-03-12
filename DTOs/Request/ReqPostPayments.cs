using System.ComponentModel.DataAnnotations;
using APIx.Models;

namespace APIx.RequestDTOs;
public class ReqPostPaymentsDTO
{
    [Required]
    public required OriginDTO Origin { get; set; }

    [Required]
    public required DestinyDTO Destiny { get; set; }

    [Required]
    public required int Amount { get; set; }

    public string? Description { get; set; }

    public enum KeyType
    {
        CPF,
        Email,
        Phone,
        Random
    }

    public class OriginDTO
    {
        [Required]
        public required UserDTO User { get; set; }

        [Required]
        public required AccountDTO Account { get; set; }
    }

    public class DestinyDTO
    {
        [Required]
        public required KeyDTO Key { get; set; }
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


    public string GetOriginUserCpf()
    {
        return Origin.User.Cpf;
    }

    public PaymentProviderAccount GetOriginPaymentProviderAccount()
    {
        return new PaymentProviderAccount(Origin.Account.Number, Origin.Account.Agency);
    }

    public PixKey GetDestinyPixKey()
    {
        return new PixKey(Destiny.Key.Type, Destiny.Key.Value);
    }

    public int GetPaymentAmount()
    {
        return Amount;
    }

    public string? GetPaymentDescription()
    {
        return Description;
    }
}