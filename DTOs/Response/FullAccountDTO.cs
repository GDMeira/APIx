using APIx.Models;

namespace APIx.ResponseDTOs;

public class FullAccountDTO(PaymentProviderAccount account)
{
    public string Number { get; set; } = account.Number;

    public string Agency { get; set; } = account.Agency;

    public string BankName { get; set; } = account.PaymentProvider.Name;

    public string BankId { get; set; } = account.PaymentProviderId.ToString();
}