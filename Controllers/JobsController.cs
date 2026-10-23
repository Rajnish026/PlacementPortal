using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlacementPortal.Data;
using PlacementPortal.Models;
using PlacementPortal.Models.ViewModels;

namespace PlacementPortal.Controllers;

public class JobsController : Controller
{
    private readonly ApplicationDbContext _context;

    public JobsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Jobs
    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm, string? jobType, string? location, bool? onlyActive)
    {
        // 1. Base query
        var query = _context.JobListings
            .AsNoTracking()
            .AsQueryable();

        // 2. Active filter (default true)
        bool filterActiveOnly = onlyActive ?? true;
        if (filterActiveOnly)
        {
            query = query.Where(j => j.IsActive);
        }

        // 3. Keyword Search (Matches Job Title or Company Name)
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var cleanTerm = searchTerm.Trim().ToLower();
            query = query.Where(j => 
                j.Title.ToLower().Contains(cleanTerm) || 
                j.CompanyName.ToLower().Contains(cleanTerm));
        }

        // 4. Job Type Filter
        if (!string.IsNullOrWhiteSpace(jobType) && !jobType.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            var cleanType = jobType.Trim().ToLower();
            query = query.Where(j => j.JobType.ToLower() == cleanType);
        }

        // 5. Location Filter
        if (!string.IsNullOrWhiteSpace(location) && !location.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            var cleanLoc = location.Trim().ToLower();
            query = query.Where(j => j.Location.ToLower().Contains(cleanLoc));
        }

        // 6. Execute query
        var jobs = await query
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();

        // 7. Distinct locations for dropdown
        var distinctLocations = await _context.JobListings
            .Select(j => j.Location)
            .Distinct()
            .OrderBy(l => l)
            .ToListAsync();

        var viewModel = new JobListViewModel
        {
            Jobs = jobs,
            SearchTerm = searchTerm,
            JobType = jobType ?? "All",
            Location = location ?? "All",
            OnlyActive = filterActiveOnly,
            AvailableLocations = distinctLocations
        };

        return View(viewModel);
    }

    // GET: /Jobs/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var job = await _context.JobListings
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == id.Value);

        if (job == null)
        {
            return NotFound();
        }

        bool hasApplied = false;
        DateTime? appliedDate = null;
        string? appStatus = null;
        double? studentCgpa = null;
        bool isEligible = true;
        string? eligibilityMsg = null;

        // If user is a logged-in student, check application and eligibility status
        if (User.Identity?.IsAuthenticated == true && User.IsInRole("Student"))
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(idClaim, out int studentId))
            {
                var student = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == studentId);

                if (student != null)
                {
                    studentCgpa = student.Cgpa;

                    var existingApp = await _context.JobApplications
                        .AsNoTracking()
                        .FirstOrDefaultAsync(a => a.JobListingId == id.Value && a.UserId == studentId);

                    if (existingApp != null)
                    {
                        hasApplied = true;
                        appliedDate = existingApp.AppliedAt;
                        appStatus = existingApp.Status;
                    }

                    if (student.Cgpa.HasValue && job.MinCgpa > 0 && student.Cgpa.Value < job.MinCgpa)
                    {
                        isEligible = false;
                        eligibilityMsg = $"Your current CGPA ({student.Cgpa.Value:F2}) is below the required minimum cutoff ({job.MinCgpa:F2}).";
                    }
                }
            }
        }

        var viewModel = new JobDetailsViewModel
        {
            Job = job,
            HasApplied = hasApplied,
            AppliedDate = appliedDate,
            ApplicationStatus = appStatus,
            IsEligible = isEligible,
            EligibilityMessage = eligibilityMsg,
            StudentCgpa = studentCgpa
        };

        return View(viewModel);
    }

    // POST: /Jobs/Apply
    [HttpPost]
    [Authorize(Roles = "Student")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(int jobListingId, string? coverNote)
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(idClaim, out int studentId))
        {
            return RedirectToAction("Login", "Account");
        }

        // 1. Verify Job Listing exists
        var job = await _context.JobListings
            .FirstOrDefaultAsync(j => j.Id == jobListingId);

        if (job == null)
        {
            return NotFound();
        }

        // 2. Prevent applications to inactive or expired listings
        if (!job.IsActive || job.Deadline.Date < DateTime.Today)
        {
            TempData["ErrorMessage"] = "Applications are closed for this opportunity as it is inactive or the application deadline has passed.";
            return RedirectToAction(nameof(Details), new { id = jobListingId });
        }

        // 3. Prevent duplicate applications
        var alreadyApplied = await _context.JobApplications
            .AnyAsync(a => a.JobListingId == jobListingId && a.UserId == studentId);

        if (alreadyApplied)
        {
            TempData["ErrorMessage"] = "You have already submitted an application for this drive.";
            return RedirectToAction(nameof(Details), new { id = jobListingId });
        }

        // 4. Save new application record
        var application = new JobApplication
        {
            JobListingId = jobListingId,
            UserId = studentId,
            AppliedAt = DateTime.UtcNow,
            Status = "Pending",
            CoverNote = string.IsNullOrWhiteSpace(coverNote) ? null : coverNote.Trim()
        };

        _context.JobApplications.Add(application);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Your application for '{job.Title}' at {job.CompanyName} was submitted successfully!";
        return RedirectToAction("MyApplications", "Student");
    }
}
