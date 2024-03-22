using APIx.Models;

namespace APIx.ResponseDTOs;

public class ResPostConcilliationDTO(Concilliation concilliation)
{
    public int Id { get; set; } = concilliation.Id;
    public string Status { get; set; } = concilliation.Status;

}