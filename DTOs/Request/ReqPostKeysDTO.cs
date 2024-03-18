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

