using APIx.Models;

namespace APIx.ResponseDTOs;

public class KeyDTOGetKeys(string type, string value)
{
    public string Type { get; set; } = type;
    public string Value { get; set; } = value;
}

public class UserDTOGetKeys(string name, string cpf)
{
    public string Name { get; set; } = name;
    public string Cpf { get; set; } = cpf;
}

public class AccountDTOGetKeys(string number, string agency, string bankName, string bankId)
{
    public string Number { get; set; } = number;

    public string Agency { get; set; } = agency;

    public string BankName { get; set; } = bankName;

    public string BankId { get; set; } = bankId;
}
public class ResGetKeysDTO(PixKey pixKey, User user,
    PaymentProviderAccount paymentProviderAccount, PaymentProvider paymentProvider)
{
    public KeyDTOGetKeys Key { get; set; } = new KeyDTOGetKeys(pixKey.Type, pixKey.Value);
    public UserDTOGetKeys User { get; set; } = new UserDTOGetKeys(user.Name, user.Cpf);
    public AccountDTOGetKeys Account { get; set; } = new AccountDTOGetKeys(paymentProviderAccount.Number,
        paymentProviderAccount.Agency, paymentProvider.Name, paymentProvider.Id.ToString());
}