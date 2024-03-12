using APIx.Models;

namespace APIx.ResponseDTOs;
public class AccountDTO(string number, string agency)
{
    public string Number { get; set; } = number;
    public string Agency { get; set; } = agency;
}