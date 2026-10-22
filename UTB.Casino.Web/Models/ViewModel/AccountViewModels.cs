using System.ComponentModel.DataAnnotations;
using UTB.Casino.Web.Validation;

namespace UTB.Casino.Web.Models.ViewModel;

public class RegisterViewModel
{
    [Required, StringLength(30, MinimumLength = 3), RegularExpression(@"^[A-Za-z0-9_.-]+$", ErrorMessage = "Use letters, digits, dot, dash or underscore.")]
    [Display(Name = "Username")]
    public string UserName { get; set; } = "";

    [Required, EmailAddress, StringLength(100)]
    public string Email { get; set; } = "";

    [Required, StringLength(60, MinimumLength = 2)]
    [Display(Name = "Display name")]
    public string DisplayName { get; set; } = "";

    [Required, DataType(DataType.Date), MinimumAge(18)]
    [Display(Name = "Date of birth")]
    public DateTime? DateOfBirth { get; set; }

    [Required, StringLength(100, MinimumLength = 8), DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Required, DataType(DataType.Password), Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = "";
}

public class LoginViewModel
{
    [Required, Display(Name = "Username")]
    public string UserName { get; set; } = "";

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
