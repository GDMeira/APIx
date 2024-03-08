using APIx.Models;

namespace APIx.ResponseDTOs;
public class ResPostKeysDTO(PixKey key, User user, PaymentProviderAccount account)
{
    public KeyDTO Key { get; set; } = new KeyDTO(key.Type, key.Value);
    public UserDTO User { get; set; } = new UserDTO(user.Cpf);
    public AccountDTO Account { get; set; } = new AccountDTO(account.Number, account.Agency);
}

public enum KeyType
{
    CPF,
    Email,
    Phone,
    Random
}

public class KeyDTO(string type, string value)
{
    public string Type { get; set; } = type;
    public string Value { get; set; } = value;
}

public class UserDTO(string cpf)
{
    public string Cpf { get; set; } = cpf;
}

public class AccountDTO(string number, string agency)
{
    public string Number { get; set; } = number;
    public string Agency { get; set; } = agency;
}