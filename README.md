# Smart Attendance System

A role-based attendance management system built with **ASP.NET Core MVC** and **SQL Server**.

## Features

- **Multi-Role Support**: Admin, Teacher, Student dashboards
- **Attendance Tracking**: Mark present/absent/late per class
- **Analytics**: Real-time attendance calculations with alerts
- **Flexible Login**: Username, Email, or Student Code
- **Activity Logging**: Complete audit trail
- **Report Export**: PDF and Excel support

---

## Project Structure

```
SmartAttendance/
├── Controllers/              # HTTP request handlers
│   ├── AccountController.cs  # Login & Authentication
│   ├── AdminController.cs    # Admin dashboard & management
│   ├── TeacherController.cs  # Attendance marking
│   ├── StudentController.cs  # Student dashboard
│   └── HomeController.cs     # Home page
│
├── Models/                   # Data models
│   ├── Entities/             # Database entities
│   │   ├── User.cs
│   │   ├── Student.cs
│   │   ├── Teacher.cs
│   │   ├── Course.cs
│   │   ├── CourseAssignment.cs
│   │   ├── Enrollment.cs
│   │   ├── AttendanceRecord.cs
│   │   ├── ActivityLog.cs
│   │   └── ErrorViewModel.cs
│   │
│   └── ViewModels/           # Request/Response models
│       ├── LoginViewModel.cs
│       ├── RegisterViewModel.cs
│       ├── AttendanceViewModel.cs
│       ├── StudentViewModel.cs
│       └── ... (other ViewModels)
│
├── Data/
│   └── AppDbContext.cs       # Entity Framework context
│
├── Repositories/             # Data access layer
│   ├── Interfaces/
│   │   ├── IRepository.cs
│   │   └── IUnitOfWork.cs
│   └── Implementations/
│       ├── Repository.cs
│       └── UnitOfWork.cs
│
├── Services/                 # Business logic layer
│   ├── Interfaces/
│   │   ├── IAttendanceService.cs
│   │   ├── IActivityLogService.cs
│   │   └── IReportService.cs
│   └── Implementations/
│       ├── AttendanceService.cs
│       ├── ActivityLogService.cs
│       └── ReportService.cs
│
├── Patterns/                 # Design patterns
│   ├── Factory/
│   │   ├── IReportExporter.cs
│   │   ├── ExcelReportExporter.cs
│   │   ├── PdfReportExporter.cs
│   │   └── ReportExporterFactory.cs
│   └── Singleton/
│       └── AppSettings.cs
│
├── Filters/
│   └── ActivityLogFilter.cs  # Audit trail logging
│
├── Views/                    # Razor templates
│   ├── Account/
│   │   ├── Login.cshtml
│   │   ├── Register.cshtml
│   │   └── Settings.cshtml
│   ├── Admin/
│   ├── Teacher/
│   ├── Student/
│   ├── Home/
│   └── Shared/
│
├── wwwroot/                  # Static files
│   ├── css/
│   ├── js/
│   └── lib/
│
├── Migrations/               # EF Core migrations
├── Database/
│   └── SmartAttendanceDB.sql # Database script
│
├── Properties/
│   └── launchSettings.json
│
├── appsettings.json          # Configuration
├── Program.cs                # Application entry point
└── SmartAttendance.csproj    # Project file
```

---

## Setup Instructions

### Prerequisites

- **.NET 8 SDK** or later
- **SQL Server** 2019 Express or above
- **SQL Server Management Studio (SSMS)** or Azure Data Studio
- **Visual Studio 2022** or VS Code

### Step 1: Clone Repository

```bash
git clone https://github.com/Syed-Rifat/SmartAttendance.git
cd SmartAttendance
```

### Step 2: Setup Database

1. Open **SQL Server Management Studio (SSMS)**
2. Connect to your SQL Server instance
3. Open `Database/SmartAttendanceDB.sql`
4. Execute the script
   - Creates `SmartAttendanceDB` database
   - Creates all tables and relationships
   - Creates views for calculations
   - Inserts default Admin user

### Step 3: Update Connection String

Edit `appsettings.json`:

```json
"ConnectionStrings": {
  "Default": "Server=localhost\\SQLEXPRESS;Database=SmartAttendanceDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

Update `Server` value to match your SQL Server instance name.

### Step 4: Run Application

**Using Visual Studio:**
- Open `SmartAttendance.sln`
- Press F5 or Ctrl+F5

**Using Command Line:**
```bash
dotnet build
dotnet run
```

Application will run at `https://localhost:xxxx`

---

## Login Credentials

### Admin Account (Default)

| Field | Value |
|-------|-------|
| **Username** | admin |
| **Email** | admin@attendance.edu |
| **Password** | Admin@123 |
| **Role** | Admin |

### Login Options

- Username: `admin`
- Email: `admin@attendance.edu`
- Student Code (for students): e.g., `CSE-2021-042`

---

## User Roles

### Administrator
- Manage courses and sections
- Manage teachers and students
- Bulk upload students
- Assign teachers to courses
- Enroll students
- View activity logs
- Configure system settings

### Teacher
- View assigned courses and students
- Mark attendance per class
- View attendance records
- Monitor low attendance students
- Export reports

### Student
- View personal attendance
- Check attendance percentage per course
- Receive low attendance alerts
- Export attendance records

---

## Technology Stack

| Component | Technology |
|-----------|-----------|
| **Framework** | ASP.NET Core MVC 8.0 |
| **Database** | SQL Server 2019+ |
| **ORM** | Entity Framework Core |
| **Password Hashing** | BCrypt.Net |
| **Frontend** | HTML5, CSS3, JavaScript, Razor Pages |
| **Architecture** | Repository Pattern + Unit of Work |

---

## Database Tables

| Table | Purpose |
|-------|---------|
| Users | Authentication & roles |
| Students | Student information |
| Teachers | Teacher information |
| Courses | Course catalog |
| CourseAssignments | Teacher-Course-Section mapping |
| Enrollments | Student-Course registration |
| AttendanceRecords | Daily attendance tracking |
| ActivityLogs | System audit trail |

---

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost\\SQLEXPRESS;Database=SmartAttendanceDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "AppSettings": {
    "MinAttendanceThreshold": 75.0
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

**Key Settings:**
- `MinAttendanceThreshold`: Attendance % for low attendance alerts (default: 75%)
- `Server`: SQL Server instance name
- `Trusted_Connection`: Use Windows authentication

---

## Troubleshooting

### Database Connection Error
- Verify SQL Server is running
- Check connection string matches your server name
- Run SQL script in SSMS
- Ensure database `SmartAttendanceDB` is created

### Login Fails
- Verify database is initialized
- Check `Users` table has admin record
- Ensure role matches selected login role

### Application Won't Start
```bash
# Clear cache and rebuild
dotnet clean
dotnet build

# Restore dependencies
dotnet restore

# Run with debug info
dotnet run --verbosity Debug
```

---

## Key Features Explained

### Attendance Marking
- Teachers mark attendance with: Present, Absent, Late
- System auto-calculates attendance percentage
- Real-time alerts for students below threshold (75%)

### Role-Based Access
- Each role has separate dashboard
- Controllers check authorization before processing
- Activity logged for all actions

### Security
- Passwords hashed with BCrypt
- Session-based authentication
- Complete activity audit trail
- SQL injection prevention via EF Core

---

## Common Tasks

### Add New Student
1. Login as Admin
2. Go to Students → Add Student
3. Fill student details
4. Assign to courses via Enrollments

### Mark Attendance
1. Login as Teacher
2. Go to My Classes
3. Select course and date
4. Mark attendance for each student
5. Submit

### View Attendance (Student)
1. Login as Student
2. Go to My Attendance
3. View attendance per course
4. Check percentage and alerts
5. Download report if needed

---

## Support

For issues or questions, refer to:
- Code comments in Controllers and Services
- Database schema in `Database/SmartAttendanceDB.sql`
- Error logs in `bin/Debug/` or `bin/Release/`

---

**Built with:** .NET 8.0 | SQL Server | Entity Framework Core | ASP.NET Core MVC

**Version:** 1.0 | **Last Updated:** April 2026
