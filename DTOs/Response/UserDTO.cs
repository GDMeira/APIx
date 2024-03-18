using APIx.Models;

namespace APIx.ResponseDTOs;

public class UserDTO(string cpf)
{
    public string Cpf { get; set; } = cpf;
}