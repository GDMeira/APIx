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

    public Payment GetPayment()
    {
        return new Payment(Amount, Description);
    }
}