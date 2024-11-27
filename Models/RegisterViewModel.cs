using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    // อีเมลของผู้ใช้
    [Required(ErrorMessage = "Please enter your email")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string? Email { get; set; }

    // รหัสผ่าน
    [Required(ErrorMessage = "Please enter your password")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    // ยืนยันรหัสผ่าน
    [Required(ErrorMessage = "Please confirm your password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string? ConfirmPassword { get; set; }
}
