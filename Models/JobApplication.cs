using System.ComponentModel.DataAnnotations;

namespace PlacementPortal.Models;

/// <summary>
/// Represents an application submitted by a student for a specific job listing.
/// </summary>
public class JobApplication
{
    public int Id { get; set; }

    [Required]
    public int JobListingId { get; set; }

    public JobListing? JobListing { get; set; }

    [Required]
    public int UserId { get; set; }

    public User? User { get; set; }

    [Display(Name = "Applied Date")]
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Application status: "Pending", "Under Review", "Shortlisted", "Selected", "Rejected"
    /// </summary>
    [Required]
    [StringLength(30)]
    [Display(Name = "Status")]
    public string Status { get; set; } = "Pending";

    [StringLength(500, ErrorMessage = "Cover statement cannot exceed 500 characters.")]
    [Display(Name = "Statement of Interest / Cover Note")]
    public string? CoverNote { get; set; }

    [StringLength(500)]
    [Display(Name = "Placement Cell Remarks")]
    public string? AdminRemarks { get; set; }
}
