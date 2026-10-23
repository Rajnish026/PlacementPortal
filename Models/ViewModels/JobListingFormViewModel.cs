using System.ComponentModel.DataAnnotations;

namespace PlacementPortal.Models.ViewModels;

public class JobListingFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Job title is required.")]
    [StringLength(120, ErrorMessage = "Job title cannot exceed 120 characters.")]
    [Display(Name = "Job / Internship Role Title")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Company name is required.")]
    [StringLength(100, ErrorMessage = "Company name cannot exceed 100 characters.")]
    [Display(Name = "Company Name")]
    public string CompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Opportunity type is required.")]
    [StringLength(30)]
    [Display(Name = "Opportunity Type")]
    public string JobType { get; set; } = "Full-Time"; // "Full-Time" or "Internship"

    [Required(ErrorMessage = "Location is required.")]
    [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters.")]
    [Display(Name = "Location / Work Mode")]
    public string Location { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role description is required.")]
    [Display(Name = "Job Description & Overview")]
    public string Description { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Requirements cannot exceed 1000 characters.")]
    [Display(Name = "Key Technical & Academic Requirements")]
    public string? Requirements { get; set; }

    [Range(0.0, 10.0, ErrorMessage = "Minimum CGPA must be between 0.0 and 10.0")]
    [Display(Name = "Minimum CGPA Cutoff (0.0 for No Bar)")]
    public double MinCgpa { get; set; } = 0.0;

    [StringLength(200, ErrorMessage = "Eligible departments cannot exceed 200 characters.")]
    [Display(Name = "Eligible Branches (e.g. CSE, IT, ECE or All Branches)")]
    public string? EligibleDepartments { get; set; } = "All Branches";

    [StringLength(80, ErrorMessage = "Package/Stipend cannot exceed 80 characters.")]
    [Display(Name = "CTC Package or Monthly Stipend (e.g. ₹8.5 LPA or ₹30,000 / month)")]
    public string? SalaryOrStipend { get; set; }

    [Required(ErrorMessage = "Application deadline is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Application Deadline")]
    public DateTime Deadline { get; set; } = DateTime.Today.AddDays(30);

    [Display(Name = "Listing Active Status")]
    public bool IsActive { get; set; } = true;
}
