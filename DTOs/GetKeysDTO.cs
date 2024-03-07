using System.ComponentModel.DataAnnotations;
using APIx.Models;

namespace APIx.DTOs;

private class KeyDTO
{
    public required string Type { get; set; }
    public required string Value { get; set; }
}

private class UserDTO
{
    public required string Name { get; set; }
    public required string Cpf { get; set; }
}

private class AccountDTO
{
    public required string Number { get; set; }

    public required string Agency { get; set; }

    public required string BankName { get; set; }

    public required string BankId { get; set; }
}
public class GetKeysDTO
{
    public required KeyDTO Key { get; set; }
    public required UserDTO User { get; set; }
    public required AccountDTO Account { get; set; }
}