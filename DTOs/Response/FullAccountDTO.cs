using APIx.Models;

namespace APIx.ResponseDTOs;

public class FullAccountDTO(string number, string agency, string bankName, string bankId)
{
    public string Number { get; set; } = number;

    public string Agency { get; set; } = agency;

    public string BankName { get; set; } = bankName;

    public string BankId { get; set; } = bankId;
}