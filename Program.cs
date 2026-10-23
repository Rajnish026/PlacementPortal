using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PlacementPortal.Data;
using PlacementPortal.Models;
using PlacementPortal.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Add MVC Controllers and Views
builder.Services.AddControllersWithViews();

// 2. Configure Entity Framework Core with SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=PlacementPortal.db";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// 3. Register secure password hasher service
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();

// 4. Configure ASP.NET Core Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "PlacementPortal.AuthCookie";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// 5. Handle CLI admin account provisioning: dotnet run -- create-admin [--email ... --password ... --name ...]
if (args.Length > 0 && args[0].Equals("create-admin", StringComparison.OrdinalIgnoreCase))
{
    using var adminScope = app.Services.CreateScope();
    var dbContext = adminScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var hasher = adminScope.ServiceProvider.GetRequiredService<IPasswordHasherService>();
    DbInitializer.Initialize(dbContext);

    string? name = null;
    string? email = null;
    string? password = null;

    for (int i = 1; i < args.Length; i++)
    {
        if (args[i].Equals("--email", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length) email = args[++i];
        else if (args[i].Equals("--password", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length) password = args[++i];
        else if (args[i].Equals("--name", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length) name = args[++i];
    }

    if (string.IsNullOrWhiteSpace(email))
    {
        Console.Write("Enter Admin Full Name: ");
        name = Console.ReadLine();
        Console.Write("Enter Admin Email Address: ");
        email = Console.ReadLine();
        Console.Write("Enter Admin Password: ");
        password = Console.ReadLine();
    }

    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        Console.WriteLine("[ERROR] Admin email and password cannot be empty.");
        return;
    }

    var normalizedEmail = email.Trim().ToLowerInvariant();
    var existingUser = dbContext.Users.FirstOrDefault(u => u.Email.ToLower() == normalizedEmail);

    if (existingUser != null)
    {
        existingUser.Role = "Admin";
        if (!string.IsNullOrWhiteSpace(name)) existingUser.FullName = name.Trim();
        existingUser.PasswordHash = hasher.HashPassword(existingUser, password);
        dbContext.SaveChanges();
        Console.WriteLine($"[SUCCESS] Existing user '{normalizedEmail}' promoted to Admin role with updated password.");
    }
    else
    {
        var admin = new User
        {
            FullName = string.IsNullOrWhiteSpace(name) ? "Placement Administrator" : name.Trim(),
            Email = normalizedEmail,
            Role = "Admin",
            CreatedAt = DateTime.UtcNow
        };
        admin.PasswordHash = hasher.HashPassword(admin, password);
        dbContext.Users.Add(admin);
        dbContext.SaveChanges();
        Console.WriteLine($"[SUCCESS] Admin account '{normalizedEmail}' created successfully.");
    }

    return;
}

// 6. Initialize Database and seed realistic sample job listings on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating or initializing the database.");
    }
}

// 7. Configure HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication MUST be positioned between UseRouting and UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
