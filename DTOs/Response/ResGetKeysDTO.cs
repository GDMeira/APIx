using APIx.Models;

namespace APIx.ResponseDTOs;
public class ResGetKeysDTO(PixKey pixKey, User user,
    PaymentProviderAccount paymentProviderAccount, PaymentProvider paymentProvider)
{
    public KeyDTO Key { get; set; } = new KeyDTO(pixKey.Type, pixKey.Value);
    public FullUserDTO User { get; set; } = new FullUserDTO(user.Name, user.Cpf);
    public FullAccountDTO Account { get; set; } = new FullAccountDTO(paymentProviderAccount.Number,
        paymentProviderAccount.Agency, paymentProvider.Name, paymentProvider.Id.ToString());
    
    public DateTime Now { get; set; } = DateTime.Now;
}