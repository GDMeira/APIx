using System.ComponentModel.DataAnnotations;

namespace APIx.RequestDTOs;

public class ReqPostConcilliationDTO
{
    [Required]
    [Url]
    public required string File { get; set; }

    public string GetFile()
    {
        return File;
    }
}