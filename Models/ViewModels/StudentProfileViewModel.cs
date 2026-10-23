using System.ComponentModel.DataAnnotations;

namespace PlacementPortal.Models.ViewModels;

public class StudentProfileViewModel
{
    public int Id { get; set; }

    [Display(Name = "College Email Address")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "University Roll Number")]
    public string RollNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Full Name is required.")]
    [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select your department.")]
    [StringLength(80)]
    [Display(Name = "Department / Branch")]
    public string Department { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select your graduation year.")]
    [Range(2023, 2035, ErrorMessage = "Please enter a valid graduation year (2023 - 2035).")]
    [Display(Name = "Year of Graduation")]
    public int? GraduationYear { get; set; }

    [Range(0.0, 10.0, ErrorMessage = "CGPA must be between 0.0 and 10.0")]
    [Display(Name = "Cumulative Grade Point Average (CGPA)")]
    public double? Cgpa { get; set; }

    [Phone(ErrorMessage = "Please enter a valid phone number.")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
    [Display(Name = "Contact Phone Number")]
    public string? PhoneNumber { get; set; }

    [StringLength(500, ErrorMessage = "Skills summary cannot exceed 500 characters.")]
    [Display(Name = "Key Technical Skills (e.g. C#, .NET, SQL, Python)")]
    public string? Skills { get; set; }

    [Url(ErrorMessage = "Please enter a valid web URL (e.g. https://drive.google.com/...)")]
    [StringLength(300, ErrorMessage = "Resume URL cannot exceed 300 characters.")]
    [Display(Name = "Online Resume / Portfolio Link")]
    public string? ResumeUrl { get; set; }
}
