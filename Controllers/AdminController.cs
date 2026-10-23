using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlacementPortal.Data;
using PlacementPortal.Models;
using PlacementPortal.Models.ViewModels;

namespace PlacementPortal.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    // Allowed application status transitions whitelist
    private static readonly HashSet<string> ValidStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Pending",
        "Under Review",
        "Shortlisted",
        "Selected",
        "Rejected"
    };

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ==========================================
    // 1. ADMIN DASHBOARD OVERVIEW
    // ==========================================

    // GET: /Admin
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var totalStudents = await _context.Users
            .CountAsync(u => u.Role == "Student");

        var activeJobs = await _context.JobListings
            .CountAsync(j => j.IsActive);

        var totalApplications = await _context.JobApplications
            .CountAsync();

        var pendingReviews = await _context.JobApplications
            .CountAsync(a => a.Status == "Pending" || a.Status == "Under Review");

        var recentApplications = await _context.JobApplications
            .AsNoTracking()
            .Include(a => a.User)
            .Include(a => a.JobListing)
            .OrderByDescending(a => a.AppliedAt)
            .Take(6)
            .ToListAsync();

        var recentJobs = await _context.JobListings
            .AsNoTracking()
            .OrderByDescending(j => j.CreatedAt)
            .Take(5)
            .ToListAsync();

        var viewModel = new AdminDashboardViewModel
        {
            TotalStudentsCount = totalStudents,
            ActiveJobListingsCount = activeJobs,
            TotalApplicationsCount = totalApplications,
            PendingReviewsCount = pendingReviews,
            RecentApplications = recentApplications,
            RecentJobListings = recentJobs
        };

        return View(viewModel);
    }

    // ==========================================
    // 2. JOB LISTINGS MANAGEMENT (CRUD)
    // ==========================================

    // GET: /Admin/Jobs
    [HttpGet]
    public async Task<IActionResult> Jobs(string? status)
    {
        var query = _context.JobListings
            .AsNoTracking()
            .Include(j => j.JobApplications)
            .AsQueryable();

        if (status == "Active")
        {
            query = query.Where(j => j.IsActive);
        }
        else if (status == "Inactive")
        {
            query = query.Where(j => !j.IsActive);
        }

        var jobs = await query
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();

        ViewData["CurrentStatusFilter"] = status ?? "All";
        return View(jobs);
    }

    // GET: /Admin/CreateJob
    [HttpGet]
    public IActionResult CreateJob()
    {
        var model = new JobListingFormViewModel
        {
            Deadline = DateTime.Today.AddDays(30),
            IsActive = true
        };
        return View(model);
    }

    // POST: /Admin/CreateJob
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateJob(JobListingFormViewModel model)
    {
        if (model.Deadline.Date < DateTime.Today)
        {
            ModelState.AddModelError(nameof(model.Deadline), "Application deadline must be today or a future date.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var job = new JobListing
        {
            Title = model.Title.Trim(),
            CompanyName = model.CompanyName.Trim(),
            JobType = model.JobType.Trim(),
            Location = model.Location.Trim(),
            Description = model.Description.Trim(),
            Requirements = string.IsNullOrWhiteSpace(model.Requirements) ? null : model.Requirements.Trim(),
            MinCgpa = model.MinCgpa,
            EligibleDepartments = string.IsNullOrWhiteSpace(model.EligibleDepartments) ? "All Branches" : model.EligibleDepartments.Trim(),
            SalaryOrStipend = string.IsNullOrWhiteSpace(model.SalaryOrStipend) ? null : model.SalaryOrStipend.Trim(),
            Deadline = model.Deadline.Date,
            IsActive = model.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        _context.JobListings.Add(job);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Campus drive '{job.Title}' at {job.CompanyName} was created successfully.";
        return RedirectToAction(nameof(Jobs));
    }

    // GET: /Admin/EditJob/5
    [HttpGet]
    public async Task<IActionResult> EditJob(int? id)
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

        var model = new JobListingFormViewModel
        {
            Id = job.Id,
            Title = job.Title,
            CompanyName = job.CompanyName,
            JobType = job.JobType,
            Location = job.Location,
            Description = job.Description,
            Requirements = job.Requirements,
            MinCgpa = job.MinCgpa,
            EligibleDepartments = job.EligibleDepartments,
            SalaryOrStipend = job.SalaryOrStipend,
            Deadline = job.Deadline,
            IsActive = job.IsActive
        };

        return View(model);
    }

    // POST: /Admin/EditJob/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditJob(int id, JobListingFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var job = await _context.JobListings.FirstOrDefaultAsync(j => j.Id == id);
        if (job == null)
        {
            return NotFound();
        }

        // Update properties
        job.Title = model.Title.Trim();
        job.CompanyName = model.CompanyName.Trim();
        job.JobType = model.JobType.Trim();
        job.Location = model.Location.Trim();
        job.Description = model.Description.Trim();
        job.Requirements = string.IsNullOrWhiteSpace(model.Requirements) ? null : model.Requirements.Trim();
        job.MinCgpa = model.MinCgpa;
        job.EligibleDepartments = string.IsNullOrWhiteSpace(model.EligibleDepartments) ? "All Branches" : model.EligibleDepartments.Trim();
        job.SalaryOrStipend = string.IsNullOrWhiteSpace(model.SalaryOrStipend) ? null : model.SalaryOrStipend.Trim();
        job.Deadline = model.Deadline.Date;
        job.IsActive = model.IsActive;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Campus drive '{job.Title}' was updated successfully.";
        return RedirectToAction(nameof(Jobs));
    }

    // POST: /Admin/ToggleJobStatus/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleJobStatus(int id)
    {
        var job = await _context.JobListings.FirstOrDefaultAsync(j => j.Id == id);
        if (job == null)
        {
            return NotFound();
        }

        job.IsActive = !job.IsActive;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Drive '{job.Title}' is now {(job.IsActive ? "Active" : "Deactivated")}.";
        return RedirectToAction(nameof(Jobs));
    }

    // ==========================================
    // 3. APPLICATION MANAGEMENT
    // ==========================================

    // GET: /Admin/Applications
    [HttpGet]
    public async Task<IActionResult> Applications(string? status, int? jobId)
    {
        var query = _context.JobApplications
            .AsNoTracking()
            .Include(a => a.User)
            .Include(a => a.JobListing)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) && !status.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(a => a.Status.ToLower() == status.Trim().ToLower());
        }

        if (jobId.HasValue && jobId.Value > 0)
        {
            query = query.Where(a => a.JobListingId == jobId.Value);
        }

        var applications = await query
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync();

        var jobListings = await _context.JobListings
            .AsNoTracking()
            .OrderBy(j => j.Title)
            .ToListAsync();

        var viewModel = new ApplicationManagementViewModel
        {
            Applications = applications,
            SelectedStatus = status ?? "All",
            SelectedJobId = jobId,
            AvailableJobListings = jobListings
        };

        return View(viewModel);
    }

    // POST: /Admin/UpdateApplicationStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateApplicationStatus(int applicationId, string status, string? remarks)
    {
        // 1. Whitelist validation
        if (string.IsNullOrWhiteSpace(status) || !ValidStatuses.Contains(status.Trim()))
        {
            TempData["ErrorMessage"] = "Invalid application status specified.";
            return RedirectToAction(nameof(Applications));
        }

        // 2. Fetch application with student details
        var application = await _context.JobApplications
            .Include(a => a.User)
            .Include(a => a.JobListing)
            .FirstOrDefaultAsync(a => a.Id == applicationId);

        if (application == null)
        {
            return NotFound();
        }

        // 3. Update status and placement cell feedback
        var previousStatus = application.Status;
        application.Status = status.Trim();
        application.AdminRemarks = string.IsNullOrWhiteSpace(remarks) ? null : remarks.Trim();

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Application for student '{application.User?.FullName}' ({application.JobListing?.CompanyName}) updated from '{previousStatus}' to '{application.Status}'.";
        return RedirectToAction(nameof(Applications));
    }

    // ==========================================
    // 4. REGISTERED STUDENTS DIRECTORY
    // ==========================================

    // GET: /Admin/Students
    [HttpGet]
    public async Task<IActionResult> Students()
    {
        var students = await _context.Users
            .AsNoTracking()
            .Where(u => u.Role == "Student")
            .Include(u => u.JobApplications)
            .OrderBy(u => u.FullName)
            .ToListAsync();

        return View(students);
    }
}
