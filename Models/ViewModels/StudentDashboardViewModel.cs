using PlacementPortal.Models;

namespace PlacementPortal.Models.ViewModels;

public class StudentDashboardViewModel
{
    public User Student { get; set; } = null!;
    public List<JobApplication> Applications { get; set; } = new();

    public int TotalApplicationsCount => Applications.Count;
    public int ShortlistedCount => Applications.Count(a => a.Status == "Shortlisted" || a.Status == "Selected");
    public int PendingCount => Applications.Count(a => a.Status == "Pending" || a.Status == "Under Review");
}
