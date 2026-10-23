using Microsoft.EntityFrameworkCore;
using PlacementPortal.Models;

namespace PlacementPortal.Data;

/// <summary>
/// Entity Framework Core database context for the Placement & Internship Portal.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<JobListing> JobListings => Set<JobListing>();
    public DbSet<JobApplication> JobApplications => Set<JobApplication>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ensure user emails are unique
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Ensure student roll numbers are unique when provided
        modelBuilder.Entity<User>()
            .HasIndex(u => u.RollNumber)
            .IsUnique();

        // Prevent duplicate applications by the same student for the same job listing
        modelBuilder.Entity<JobApplication>()
            .HasIndex(a => new { a.JobListingId, a.UserId })
            .IsUnique();

        // Cascade delete applications when a job listing is removed
        modelBuilder.Entity<JobApplication>()
            .HasOne(a => a.JobListing)
            .WithMany(j => j.JobApplications)
            .HasForeignKey(a => a.JobListingId)
            .OnDelete(DeleteBehavior.Cascade);

        // Cascade delete applications when a student account is removed
        modelBuilder.Entity<JobApplication>()
            .HasOne(a => a.User)
            .WithMany(u => u.JobApplications)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
