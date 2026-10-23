namespace PlacementPortal.Models.ViewModels;

public class JobListViewModel
{
    public IEnumerable<JobListing> Jobs { get; set; } = new List<JobListing>();

    // Search and filter parameters
    public string? SearchTerm { get; set; }
    public string? JobType { get; set; }
    public string? Location { get; set; }
    public bool OnlyActive { get; set; } = true;

    // Filter dropdown options
    public List<string> AvailableLocations { get; set; } = new();
    public List<string> AvailableJobTypes { get; set; } = new() { "All", "Full-Time", "Internship" };

    // Metadata
    public int TotalResultsCount => Jobs.Count();
}
