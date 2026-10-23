using PlacementPortal.Models;

namespace PlacementPortal.Models.ViewModels;

public class JobDetailsViewModel
{
    public JobListing Job { get; set; } = null!;
    public bool HasApplied { get; set; }
    public DateTime? AppliedDate { get; set; }
    public string? ApplicationStatus { get; set; }
    public bool IsEligible { get; set; } = true;
    public string? EligibilityMessage { get; set; }
    public double? StudentCgpa { get; set; }
}
