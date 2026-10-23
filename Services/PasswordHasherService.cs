using Microsoft.AspNetCore.Identity;
using PlacementPortal.Models;

namespace PlacementPortal.Services;

/// <summary>
/// Service contract for cryptographically hashing and verifying user passwords using ASP.NET Core Identity standards.
/// </summary>
public interface IPasswordHasherService
{
    string HashPassword(User user, string plainPassword);
    bool VerifyPassword(User user, string hashedPassword, string providedPassword);
}

/// <summary>
/// Implements standard PBKDF2 with HMAC-SHA256 password hashing.
/// </summary>
public class PasswordHasherService : IPasswordHasherService
{
    private readonly PasswordHasher<User> _hasher = new();

    public string HashPassword(User user, string plainPassword)
    {
        return _hasher.HashPassword(user, plainPassword);
    }

    public bool VerifyPassword(User user, string hashedPassword, string providedPassword)
    {
        var result = _hasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}
