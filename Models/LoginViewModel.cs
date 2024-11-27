using System.ComponentModel.DataAnnotations;

public class LoginViewModel
{
    [Required(ErrorMessage = "Please enter your email")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string? Email { get; set; } // ?????

    [Required(ErrorMessage = "Please enter your password")]
    [DataType(DataType.Password)]
    public string? Password { get; set; } // ????????
}
