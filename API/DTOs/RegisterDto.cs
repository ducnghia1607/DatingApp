using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDto
{
    [Required]
    public string Username { set; get; }
    [Required]
    public string Password { set; get; }

    [Required]
    public string KnownAs { set; get; }

    [Required]
    public DateOnly? DateOfBirth { set; get; }
    [Required]
    public string Gender { set; get; }

    [Required]
    public string Country { set; get; }
    [Required]
    public string City { set; get; }


}
