# Student Placement & Internship Portal

> A full-stack, enterprise-grade web application built using **ASP.NET Core 8.0 MVC**, **Entity Framework Core 8.0**, and **SQLite**, designed for college Training & Placement (T&P) cells to automate campus recruitment drives, student job applications, and placement administration.

---

## Table of Contents
1. [Project Overview](#project-overview)
2. [Key Features](#key-features)
   - [Student Module](#student-module)
   - [Job & Internship Catalog](#job--internship-catalog)
   - [Admin Management Console](#admin-management-console)
   - [Security & Architecture](#security--architecture)
3. [Technology Stack](#technology-stack)
4. [Prerequisites](#prerequisites)
5. [Installation & Setup](#installation--setup)
6. [Secure Admin Account Creation](#secure-admin-account-creation)
7. [Running the Application](#running-the-application)
8. [Automated Testing Suite](#automated-testing-suite)
9. [Project Structure](#project-structure)
10. [Academic Submission Details](#academic-submission-details)

---

## Project Overview

The **Student Placement & Internship Portal** streamlines college campus placements by replacing manual tracking sheets and disparate forms with a centralized, responsive web platform. It establishes clear role separation between **Students** applying for corporate opportunities and **Placement Officers (Admins)** coordinating campus drives, shortlisting candidates, and tracking placement statistics.

---

## Key Features

### Student Module
- **Self-Registration & Authentication:** Secure account creation requiring college roll number, passing year, branch, and email with instant duplicate prevention.
- **Student Placement Profile:** View and update personal academic profile, contact number, key technical skills, and resume/portfolio links. College roll number and email are verified and kept strictly read-only.
- **One-Click Job Applications:** Logged-in students can apply to active drives with an optional cover note/statement of interest.
- **Strict Duplicate Application Blocking:** Enforced both in business logic and at the database schema level via unique constraints.
- **My Applications Tracker:** Real-time personal tracker showing applied drives, dates, current review status (`Pending`, `Under Review`, `Shortlisted`, `Selected`, `Rejected`), and personal remarks from the placement cell.

### Job & Internship Catalog
- **Unified Opportunities Catalog:** Responsive Bootstrap card layout displaying company name, role title, job type, work location, package/stipend, eligibility cutoff, and application deadline.
- **Multi-Parameter Search & Filtering:**
  - Real-time keyword search across role titles and company names.
  - Opportunity type filter (`All`, `Full-Time`, `Internship`).
  - Location filter with dynamically populated distinct locations.
  - Active-only status toggle switch.
  - Preserved search state across pagination and requests.
- **Detailed Drive Specification:** Complete overview of roles, company overview, minimum CGPA eligibility cutoffs, eligible branches, and active application states.

### Admin Management Console
- **Protected Access:** Restricted entirely via `[Authorize(Roles = "Admin")]`. Students and visitors attempting access are blocked and redirected to Access Denied.
- **Dashboard KPI Metrics:** Real-time metric cards for:
  - Total Enrolled Students
  - Active Placement Drives
  - Total Applications Received
  - Pending Review Queue
  - Recent candidate applications and newest drives.
- **Drive Lifecycle Management (CRUD):**
  - Post new corporate recruitment drives with server-side validation.
  - Edit existing opportunities, deadlines, or eligibility parameters.
  - Soft deactivation/activation toggle to conclude drives without deleting historical student applications.
- **Application Review Workflow:**
  - View candidate applications with filters by status and company.
  - Update candidate review stages (`Pending`, `Under Review`, `Shortlisted`, `Selected`, `Rejected`) using a validated status whitelist.
  - Provide placement cell feedback and interview instructions visible directly on the student's dashboard.
- **Candidate Student Directory:** Searchable directory showing student roll numbers, branches, CGPA badges, skills, and application counts.

### Security & Architecture
- **Cryptographic Password Hashing:** Uses industry-standard PBKDF2 with HMAC-SHA256 and unique 128-bit cryptographically secure salts (`PasswordHasherService`). Plain-text passwords are never stored or logged.
- **No Hardcoded Credentials:** Zero hardcoded passwords or default admin seeds in source code or database migrations.
- **Antiforgery Protection:** Every POST endpoint enforces ASP.NET Core `[ValidateAntiForgeryToken]`.
- **Ownership Enforcement:** Students can only access and update their own profiles and view their own applications.
- **Local Redirect Protection:** Safe login redirection preventing open-redirect phishing attacks.

### Responsive Modern UI Design
- **Optimized for Common Laptop Displays:** Tailored specifically for default 100% zoom on 1280×720, 1366×768, 1440×900, and 1920×1080 screens.
- **Fluid Typography:** Uses CSS `clamp()` for dynamic hero and section headers that scale seamlessly without overflowing or pushing CTAs offscreen.
- **Constrained Layouts:** Custom container constraints and sticky sidebars (`max-height: calc(100vh - 96px); overflow-y: auto`) ensure drive overviews and action buttons remain reachable on short-viewport displays.
- **Zero Horizontal Scrolling:** Tables utilize responsive wrappers with min-width safety utilities (`.table-min-width-md/lg/xl`) and sleek custom scrollbars.

---

## Technology Stack

| Layer | Technology |
| :--- | :--- |
| **Language** | C# 12 (.NET 8.0 LTS) |
| **Framework** | ASP.NET Core 8.0 MVC |
| **ORM / Data Access** | Entity Framework Core 8.0 (Code-First) |
| **Database** | SQLite (`PlacementPortal.db`) |
| **Frontend UI** | HTML5, CSS3, Bootstrap 5.3, Bootstrap Icons |
| **Typography** | Google Fonts (*Plus Jakarta Sans*) |
| **Authentication** | ASP.NET Core Cookie Authentication with Claims-based RBAC |

---

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or higher installed on your system.
- Supported OS: Windows 10/11, macOS, or Linux.
- Optional IDE: Visual Studio 2022, Visual Studio Code (with C# Dev Kit), or JetBrains Rider.

Verify your installed .NET SDK version:
```powershell
dotnet --version
# Expected: 8.0.xxx or higher
```

---

## Installation & Setup

1. **Clone the Repository:**
   ```powershell
   git clone https://github.com/Rajnish026/PlacementPortal.git
   cd PlacementPortal
   ```

2. **Restore NuGet Packages:**
   ```powershell
   dotnet restore
   ```

3. **Build the Solution:**
   ```powershell
   dotnet build
   ```

4. **Database Initialization:**
   The SQLite database (`PlacementPortal.db`) and its schema are automatically verified and migrated on application startup. Realistic sample recruitment drives (Microsoft, Amazon, TCS, Deloitte, Google, and Infosys) are populated if the database is fresh.

---

## Secure Admin Account Creation

To comply with enterprise security practices, public registration **never** assigns the `Admin` role, and no default admin accounts are seeded in migrations.

Create the initial administrator account via the built-in Command-Line Setup:

```powershell
dotnet run -- create-admin
```

The CLI prompts for:
- **Admin Full Name** (optional, defaults to "Placement Administrator" if left blank)
- **Admin Email Address** (e.g. `admin@college.edu`)
- **Admin Password** (entered via a **hidden console prompt** where keystrokes are masked to prevent exposure on-screen or in shell history)

You can also optionally supply the name and email via CLI arguments while entering the password securely at the hidden prompt:
```powershell
dotnet run -- create-admin --name "Placement Head" --email "admin@college.edu"
```

> **Security Note:** Command-line password arguments are intentionally disabled to ensure credentials never persist in shell history or process arguments. Passwords are always entered via the hidden prompt and securely hashed using PBKDF2 with HMAC-SHA512.
>
> *Note: If the email already belongs to an existing user, the CLI securely promotes that user to the `Admin` role and updates their password hash.*

---

## Running the Application

Start the local web server:
```powershell
dotnet run --launch-profile http
```

Once started, open your browser and navigate to:
```
http://localhost:5247
```

### Default Navigation Paths:
- **Home / Landing:** `http://localhost:5247/`
- **Browse Opportunities:** `http://localhost:5247/Jobs`
- **Student Register:** `http://localhost:5247/Account/Register`
- **Sign In:** `http://localhost:5247/Account/Login`
- **Student Dashboard:** `http://localhost:5247/Student`
- **Student Profile:** `http://localhost:5247/Student/Profile`
- **My Applications:** `http://localhost:5247/Student/MyApplications`
- **Admin Console:** `http://localhost:5247/Admin`
- **Admin Drive Management:** `http://localhost:5247/Admin/Jobs`
- **Admin Applications Review:** `http://localhost:5247/Admin/Applications`
- **Admin Candidate Directory:** `http://localhost:5247/Admin/Students`

---

## Verification & Testing

The project is structured for easy verification and test validation using standard .NET CLI commands:

### 1. Build Verification
Verify compilation, type safety, EF Core configurations, and Razor view syntax:
```powershell
dotnet build
```

### 2. Admin Provisioning Verification
Verify database seeding, migration integrity, and hidden password hashing:
```powershell
dotnet run -- create-admin
```

### 3. Application Execution & Functional Testing
Launch the development server to verify end-to-end user workflows:
```powershell
dotnet run --launch-profile http
```

### Verified Functional Test Scenarios:
- **Authentication & Authorization:** Student registration, unique roll/email constraint validation, PBKDF2 password hashing, secure cookie session handling, and role-based route protection (`[Authorize(Roles = "Admin")]`).
- **Job Catalog & Search:** Live drive browsing, case-insensitive keyword search, filter combinations (job type, location, active status), and card responsive layouts.
- **Student Profile & Application:** Profile updates with server-side validation, single-application enforcement (duplicate prevention), deadline validation, and application tracking under My Applications.
- **Admin Management:** Drive creation, editing, soft deactivation, applicant list inspection, application status transitions (*Under Review*, *Shortlisted*, *Selected*, *Rejected*) with optional feedback notes, and student directory lookup.

---

## Project Structure

```
PlacementPortal/
├── Controllers/
│   ├── AccountController.cs       # Registration, Login, Logout, AccessDenied
│   ├── AdminController.cs         # Admin Dashboard, Job CRUD, Applications, Students
│   ├── HomeController.cs          # Public Landing page and Privacy Policy
│   ├── JobsController.cs          # Job Catalog, Search/Filter, Details, Apply
│   └── StudentController.cs       # Student Dashboard, Profile Settings, MyApplications
├── Data/
│   ├── ApplicationDbContext.cs    # EF Core DbContext with relationship & index mappings
│   └── DbInitializer.cs           # Database migration & sample recruitment drive seeder
├── Migrations/                    # EF Core SQLite schema migrations
├── Models/
│   ├── JobApplication.cs          # Application entity with status and admin remarks
│   ├── JobListing.cs              # Recruitment drive entity with eligibility rules
│   ├── User.cs                    # Student/Admin entity with hashed password & profile
│   └── ViewModels/                # Strongly-typed models with validation attributes
├── Services/
│   └── PasswordHasherService.cs   # Cryptographic PBKDF2 password hasher & verifier
├── Views/
│   ├── Account/                   # Login, Register, AccessDenied views
│   ├── Admin/                     # Index, Jobs, CreateJob, EditJob, Applications, Students
│   ├── Home/                      # Index, Privacy views
│   ├── Jobs/                      # Index, Details views
│   ├── Shared/                    # _Layout, _ValidationScriptsPartial, Error
│   └── Student/                   # Index, Profile, MyApplications views
├── wwwroot/                       # Static CSS, JS, Bootstrap, and icons
├── appsettings.json               # Database connection strings and logging levels
├── Program.cs                     # DI services, authentication, middleware, and CLI commands
├── .gitignore                     # Git ignore rules for .NET, SQLite, and binaries
└── README.md                      # Comprehensive project documentation
```

---

## Academic Submission Details

- **Course:** B.Tech Computer Science & Engineering / Information Technology
- **Subject:** Web Technology with .NET / Advanced Web Development
- **Project Title:** Student Placement & Internship Portal
- **Architecture:** ASP.NET Core Model-View-Controller (MVC) with Entity Framework Core Code-First
- **Database:** SQLite (Relational Database Management System)
