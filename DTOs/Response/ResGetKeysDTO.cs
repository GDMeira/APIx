using APIx.Models;

namespace APIx.ResponseDTOs;
public class ResGetKeysDTO(PixKey pixKey)
{
    public KeyDTO Key { get; set; } = new KeyDTO(pixKey);
    public FullUserDTO User { get; set; } = new FullUserDTO(pixKey.PaymentProviderAccount.User);
    public FullAccountDTO Account { get; set; } = new FullAccountDTO(pixKey.PaymentProviderAccount);
}