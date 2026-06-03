using System.ComponentModel.DataAnnotations;

namespace ElyraBd.Web.ViewModels.Account;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Full name is required")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 200 characters")]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Enter a valid email address")]
    [Display(Name = "Email address")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Enter a valid phone number")]
    [Display(Name = "Phone (optional)")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
    [DataType(DataType.Password)]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).+$",
        ErrorMessage = "Password must include upper, lower, digit, and special character")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please confirm your password")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
