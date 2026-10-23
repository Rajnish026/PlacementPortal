using System.ComponentModel.DataAnnotations;

namespace PlacementPortal.Models;

/// <summary>
/// Represents a user in the portal (either a Student or an Admin).
/// </summary>
public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Full Name is required.")]
    [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [StringLength(120)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Cryptographically secure password hash (never stored in plain text).
    /// </summary>
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Role { get; set; } = "Student"; // "Student" or "Admin"

    [StringLength(30)]
    [Display(Name = "College Roll Number")]
    public string? RollNumber { get; set; }

    [StringLength(80)]
    [Display(Name = "Department / Branch")]
    public string? Department { get; set; }

    [Range(0.0, 10.0, ErrorMessage = "CGPA must be between 0.0 and 10.0")]
    [Display(Name = "Cumulative GPA (CGPA)")]
    public double? Cgpa { get; set; }

    [Range(2020, 2035, ErrorMessage = "Please enter a valid passing year.")]
    [Display(Name = "Graduation Year")]
    public int? GraduationYear { get; set; }

    [Phone(ErrorMessage = "Please enter a valid phone number.")]
    [StringLength(20)]
    [Display(Name = "Contact Number")]
    public string? PhoneNumber { get; set; }

    [StringLength(500)]
    [Display(Name = "Key Technical Skills")]
    public string? Skills { get; set; }

    [StringLength(300)]
    [Display(Name = "Resume / Portfolio URL")]
    public string? ResumeUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property: One student can apply to multiple jobs/internships
    public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
}
