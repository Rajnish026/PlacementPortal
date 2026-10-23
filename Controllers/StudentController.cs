using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlacementPortal.Data;
using PlacementPortal.Models;
using PlacementPortal.Models.ViewModels;

namespace PlacementPortal.Controllers;

[Authorize(Roles = "Student")]
public class StudentController : Controller
{
    private readonly ApplicationDbContext _context;

    public StudentController(ApplicationDbContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(idClaim, out int userId))
        {
            return userId;
        }
        throw new InvalidOperationException("Authenticated user has no valid identifier claim.");
    }

    // GET: /Student
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = GetCurrentUserId();

        var student = await _context.Users
            .Include(u => u.JobApplications)
                .ThenInclude(a => a.JobListing)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (student == null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        var applications = student.JobApplications
            .OrderByDescending(a => a.AppliedAt)
            .ToList();

        var viewModel = new StudentDashboardViewModel
        {
            Student = student,
            Applications = applications
        };

        return View(viewModel);
    }

    // GET: /Student/MyApplications
    [HttpGet]
    public async Task<IActionResult> MyApplications()
    {
        var userId = GetCurrentUserId();

        var applications = await _context.JobApplications
            .AsNoTracking()
            .Include(a => a.JobListing)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.AppliedAt)
            .ToListAsync();

        return View(applications);
    }

    // GET: /Student/Profile
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var userId = GetCurrentUserId();

        var student = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (student == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var viewModel = new StudentProfileViewModel
        {
            Id = student.Id,
            Email = student.Email,
            RollNumber = student.RollNumber ?? string.Empty,
            FullName = student.FullName,
            Department = student.Department ?? string.Empty,
            GraduationYear = student.GraduationYear,
            Cgpa = student.Cgpa,
            PhoneNumber = student.PhoneNumber,
            Skills = student.Skills,
            ResumeUrl = student.ResumeUrl
        };

        return View(viewModel);
    }

    // POST: /Student/Profile
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(StudentProfileViewModel model)
    {
        var currentUserId = GetCurrentUserId();

        if (!ModelState.IsValid)
        {
            // Preserve read-only fields on validation error
            var currentStudent = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == currentUserId);

            if (currentStudent != null)
            {
                model.Email = currentStudent.Email;
                model.RollNumber = currentStudent.RollNumber ?? string.Empty;
            }

            return View(model);
        }

        // Fetch student entity for update (enforcing user ownership)
        var student = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == currentUserId);

        if (student == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Update editable fields (Email and RollNumber are strictly preserved)
        student.FullName = model.FullName.Trim();
        student.Department = model.Department.Trim();
        student.GraduationYear = model.GraduationYear;
        student.Cgpa = model.Cgpa;
        student.PhoneNumber = string.IsNullOrWhiteSpace(model.PhoneNumber) ? null : model.PhoneNumber.Trim();
        student.Skills = string.IsNullOrWhiteSpace(model.Skills) ? null : model.Skills.Trim();
        student.ResumeUrl = string.IsNullOrWhiteSpace(model.ResumeUrl) ? null : model.ResumeUrl.Trim();

        await _context.SaveChangesAsync();

        // Refresh authentication cookie with updated FullName and Department
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, student.Id.ToString()),
            new Claim(ClaimTypes.Name, student.FullName),
            new Claim(ClaimTypes.Email, student.Email),
            new Claim(ClaimTypes.Role, student.Role),
            new Claim("RollNumber", student.RollNumber ?? string.Empty),
            new Claim("Department", student.Department ?? string.Empty)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));

        TempData["SuccessMessage"] = "Your student profile has been updated successfully.";
        return RedirectToAction(nameof(Profile));
    }
}
