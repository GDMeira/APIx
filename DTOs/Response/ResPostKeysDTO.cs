using APIx.Models;

namespace APIx.ResponseDTOs;
public class ResPostKeysDTO(PixKey key, User user, PaymentProviderAccount account)
{
    public KeyDTO Key { get; set; } = new KeyDTO(key.Type, key.Value);
    public UserDTO User { get; set; } = new UserDTO(user.Cpf);
    public AccountDTO Account { get; set; } = new AccountDTO(account.Number, account.Agency);
}