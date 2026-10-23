using System.ComponentModel.DataAnnotations;

namespace PlacementPortal.Models.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Full Name is required.")]
    [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [StringLength(120, ErrorMessage = "Email cannot exceed 120 characters.")]
    [Display(Name = "College Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "College Roll Number is required.")]
    [StringLength(30, ErrorMessage = "Roll number cannot exceed 30 characters.")]
    [Display(Name = "College Roll Number")]
    public string RollNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select your department / branch.")]
    [StringLength(80)]
    [Display(Name = "Department / Branch")]
    public string Department { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select your passing out year.")]
    [Range(2023, 2035, ErrorMessage = "Please enter a valid graduation year (2023 - 2035).")]
    [Display(Name = "Year of Graduation")]
    public int? GraduationYear { get; set; }
}
