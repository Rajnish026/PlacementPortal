using Microsoft.EntityFrameworkCore;
using PlacementPortal.Models;

namespace PlacementPortal.Data;

/// <summary>
/// Handles database creation and populates initial sample data for college demonstration.
/// </summary>
public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Ensure SQLite database and migrations are applied
        context.Database.Migrate();

        // Check if sample job listings already exist
        if (context.JobListings.Any())
        {
            // Ensure there is at least one closed/expired drive for verification & testing
            if (!context.JobListings.Any(j => !j.IsActive || j.Deadline < DateTime.Today))
            {
                context.JobListings.Add(new JobListing
                {
                    Title = "Associate Systems Engineer (Concluded Drive)",
                    CompanyName = "Infosys Limited",
                    JobType = "Full-Time",
                    Location = "Mysore / Bangalore",
                    Description = "Off-campus recruitment drive from the previous semester cycle. This drive is now archived and no longer accepting applications.",
                    Requirements = "B.Tech in all engineering disciplines. Minimum 6.0 CGPA.",
                    MinCgpa = 6.0,
                    EligibleDepartments = "All Engineering Branches",
                    SalaryOrStipend = "₹4,50,000 / year",
                    Deadline = DateTime.Today.AddDays(-20),
                    IsActive = false,
                    CreatedAt = DateTime.UtcNow.AddMonths(-1)
                });
                context.SaveChanges();
            }
            return;
        }

        var sampleJobs = new List<JobListing>
        {
            new JobListing
            {
                Title = "Graduate Software Development Engineer",
                CompanyName = "Microsoft Corporation",
                JobType = "Full-Time",
                Location = "Hyderabad / Bangalore",
                Description = "Join our Azure Engineering team as a full-time Graduate Software Development Engineer. You will design, develop, and deploy cloud-scale services, write clean C# and .NET code, and collaborate in an agile cross-functional team.",
                Requirements = "Strong proficiency in Data Structures, Algorithms, Object-Oriented Programming (C# / Java / C++), and fundamentals of operating systems and relational databases.",
                MinCgpa = 7.5,
                EligibleDepartments = "CSE, IT, ECE",
                SalaryOrStipend = "₹16,50,000 / year",
                Deadline = DateTime.Today.AddDays(30),
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new JobListing
            {
                Title = "Software Development Engineer Intern",
                CompanyName = "Amazon Development Centre",
                JobType = "Internship",
                Location = "Bangalore, Karnataka",
                Description = "6-month internship with potential for full-time conversion (PPO). Interns will work directly on customer-facing microservices and scalable distributed architectures.",
                Requirements = "Solid knowledge of Java, C#, or Python, good problem-solving ability, and basic understanding of RESTful APIs.",
                MinCgpa = 7.0,
                EligibleDepartments = "CSE, IT, ECE, EE",
                SalaryOrStipend = "₹50,000 / month",
                Deadline = DateTime.Today.AddDays(20),
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-4)
            },
            new JobListing
            {
                Title = "Digital Specialist Engineer",
                CompanyName = "Tata Consultancy Services (TCS)",
                JobType = "Full-Time",
                Location = "Pan-India / Multiple Locations",
                Description = "Campus recruitment drive for TCS Digital Cadre. Work on cutting-edge technologies including Cloud computing, AI/ML, full-stack enterprise development, and IoT solutions.",
                Requirements = "No active backlogs. Knowledge of modern programming languages (C#, Python, Java) and web development technologies (HTML, CSS, JavaScript).",
                MinCgpa = 6.5,
                EligibleDepartments = "All Engineering Branches",
                SalaryOrStipend = "₹7,20,000 / year",
                Deadline = DateTime.Today.AddDays(45),
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new JobListing
            {
                Title = "Data & Analytics Associate",
                CompanyName = "Deloitte India",
                JobType = "Full-Time",
                Location = "Gurgaon / Hyderabad",
                Description = "Work with enterprise clients to build modern data warehouses, business intelligence dashboards, and predictive models using SQL, Python, and cloud analytics tools.",
                Requirements = "Good communication skills, strong proficiency in SQL and relational database concepts, familiarity with Excel and data visualization.",
                MinCgpa = 6.8,
                EligibleDepartments = "CSE, IT, Mathematics & Computing",
                SalaryOrStipend = "₹8,00,000 / year",
                Deadline = DateTime.Today.AddDays(25),
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new JobListing
            {
                Title = "Network Software Intern",
                CompanyName = "Cisco Systems",
                JobType = "Internship",
                Location = "Bangalore, Karnataka",
                Description = "Work with Cisco's enterprise networking and security teams. Hands-on experience with network automation, Python/C# scripting, and Linux-based systems.",
                Requirements = "Strong foundation in Computer Networks (TCP/IP, OSI model), basic scripting in Python or C#, and enthusiasm for network engineering.",
                MinCgpa = 7.0,
                EligibleDepartments = "CSE, IT, ECE",
                SalaryOrStipend = "₹40,000 / month",
                Deadline = DateTime.Today.AddDays(15),
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new JobListing
            {
                Title = "Product Engineering Intern (Web & Backend)",
                CompanyName = "Zomato",
                JobType = "Internship",
                Location = "Gurgaon / Hybrid",
                Description = "Fast-paced internship building high-traffic consumer web features and backend API services. Work alongside senior engineers on real-world production systems.",
                Requirements = "Hands-on experience building web apps with ASP.NET Core, Node.js, or similar framework, plus basic frontend skills (JavaScript, HTML, CSS).",
                MinCgpa = 6.5,
                EligibleDepartments = "CSE, IT",
                SalaryOrStipend = "₹35,000 / month",
                Deadline = DateTime.Today.AddDays(18),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        context.JobListings.AddRange(sampleJobs);
        context.SaveChanges();
    }
}
