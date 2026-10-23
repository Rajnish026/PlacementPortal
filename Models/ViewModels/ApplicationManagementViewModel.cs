using PlacementPortal.Models;

namespace PlacementPortal.Models.ViewModels;

public class ApplicationManagementViewModel
{
    public List<JobApplication> Applications { get; set; } = new();

    public string? SelectedStatus { get; set; } = "All";
    public int? SelectedJobId { get; set; }

    public List<JobListing> AvailableJobListings { get; set; } = new();

    public int TotalCount => Applications.Count;
    public int PendingCount => Applications.Count(a => a.Status == "Pending" || a.Status == "Under Review");
    public int ShortlistedCount => Applications.Count(a => a.Status == "Shortlisted");
    public int SelectedCount => Applications.Count(a => a.Status == "Selected");
    public int RejectedCount => Applications.Count(a => a.Status == "Rejected");
}
