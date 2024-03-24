using APIx.Models;

namespace APIx.ResponseDTOs;
public class ResPostKeysDTO(PixKey key, User user)
{
    public KeyDTO Key { get; set; } = new KeyDTO(key);
    public UserDTO User { get; set; } = new UserDTO(user.Cpf);
    public AccountDTO Account { get; set; } = 
        new AccountDTO(key.PaymentProviderAccount.Number, key.PaymentProviderAccount.Agency);
}