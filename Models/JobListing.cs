using System.ComponentModel.DataAnnotations;

namespace PlacementPortal.Models;

/// <summary>
/// Represents a job or internship opportunity posted by the placement cell.
/// </summary>
public class JobListing
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Job title is required.")]
    [StringLength(120, ErrorMessage = "Job title cannot exceed 120 characters.")]
    [Display(Name = "Job / Internship Role")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Company name is required.")]
    [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters.")]
    [Display(Name = "Company Name")]
    public string CompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select opportunity type.")]
    [StringLength(30)]
    [Display(Name = "Opportunity Type")]
    public string JobType { get; set; } = "Full-Time"; // "Internship" or "Full-Time"

    [Required(ErrorMessage = "Job location is required.")]
    [StringLength(100)]
    [Display(Name = "Location")]
    public string Location { get; set; } = string.Empty;

    [Required(ErrorMessage = "Job description is required.")]
    [Display(Name = "Role Description")]
    public string Description { get; set; } = string.Empty;

    [StringLength(1000)]
    [Display(Name = "Eligibility & Key Requirements")]
    public string? Requirements { get; set; }

    [Range(0.0, 10.0, ErrorMessage = "Minimum CGPA must be between 0.0 and 10.0")]
    [Display(Name = "Minimum CGPA Required")]
    public double MinCgpa { get; set; } = 0.0;

    [StringLength(200)]
    [Display(Name = "Eligible Branches")]
    public string? EligibleDepartments { get; set; } // e.g. "CSE, IT, ECE"

    [StringLength(80)]
    [Display(Name = "Package / Monthly Stipend")]
    public string? SalaryOrStipend { get; set; } // e.g. "₹8.5 LPA" or "₹25,000 / month"

    [Required(ErrorMessage = "Application deadline is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Application Deadline")]
    public DateTime Deadline { get; set; }

    [Display(Name = "Active Drive")]
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property: One job listing can receive multiple applications
    public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
}
