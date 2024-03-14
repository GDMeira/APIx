using APIx.Models;

namespace APIx.ResponseDTOs;
public class ResPostPaymentsDTO(Payment payment) 
{
    public OriginDTO Origin { get; set; } = new OriginDTO(payment.PaymentProviderAccount);

    public DestinyDTO Destiny { get; set; } = new DestinyDTO(payment.PixKey);
    public int Id { get; set; } = payment.Id;
    public DateTime CreatedAt { get; set; } = payment.CreatedAt;

    public int Amount { get; set; } = payment.Amount;

    public string? Description { get; set; } = payment.Description;

    public string Status { get; set; } = payment.Status;

    public class OriginDTO(PaymentProviderAccount account)
    {
        public AccountDTO Account { get; set; } = new AccountDTO(account.Number, account.Agency);
        public UserDTO User { get; set; } = new UserDTO(account.User.Cpf);
    }

    public class DestinyDTO(PixKey key)
    {
        public KeyDTO Key { get; set; } = new KeyDTO(key.Type, key.Value);
    }
}