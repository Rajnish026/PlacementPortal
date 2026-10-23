using PlacementPortal.Models;

namespace PlacementPortal.Models.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalStudentsCount { get; set; }
    public int ActiveJobListingsCount { get; set; }
    public int TotalApplicationsCount { get; set; }
    public int PendingReviewsCount { get; set; }

    public List<JobApplication> RecentApplications { get; set; } = new();
    public List<JobListing> RecentJobListings { get; set; } = new();
}
