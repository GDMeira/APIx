using System.ComponentModel.DataAnnotations;
using APIx.Exceptions;

namespace APIx.RequestDTOs;

public class ReqPostConcilliationDTO
{
    [Required]
    [Url]
    public required string File { get; set; }

    [Required]
    [Url]
    public required string PostBack { get; set; }

    [Required]
    [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "O campo Date deve estar no formato 'yyyy-MM-dd'")]
    public required string Date { get; set; }

    public string GetFile()
    {
        return File;
    }

    public string GetPostback()
    {
        return PostBack;
    }

    public DateOnly GetDate()
    {
        try
        {
            var date = DateOnly.Parse(Date);
            if (date.CompareTo(DateOnly.FromDateTime(DateTime.UtcNow)) > 0)
            {
                throw new UnprocessableEntryException("Date cannot be in the future.");
            }

            return date;
        }
        catch (Exception e)
        {
            throw new UnprocessableEntryException(e.Message);
        }
        
    }
}