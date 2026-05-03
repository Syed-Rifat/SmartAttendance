# Smart Attendance System

Role-based attendance management system built with **ASP.NET Core MVC** and **SQL Server**.

---

## Quick Start

### Prerequisites
- .NET 8 SDK
- SQL Server 2019+
- Visual Studio 2022 / VS Code

### Setup (4 Steps)

1. **Clone Repository**
   ```bash
   git clone https://github.com/Syed-Rifat/SmartAttendance.git
   cd SmartAttendance
   ```

2. **Setup Database**
   - Open SQL Server Management Studio
   - Execute `Database/SmartAttendanceDB.sql`

3. **Update Connection String** in `appsettings.json`
   ```json
   "ConnectionStrings": {
     "Default": "Server=localhost\\SQLEXPRESS;Database=SmartAttendanceDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
   }
   ```

4. **Run Application**
   ```bash
   dotnet build
   dotnet run
   ```

---

## Login Credentials

```
Username: admin
Email: admin@attendance.edu
Password: Admin@123
Role: Admin
```

---

## Project Structure

```
SmartAttendance/
├── Controllers/       # API endpoints
├── Models/           # Database models & ViewModels
├── Data/             # Database context
├── Repositories/     # Data access layer
├── Services/         # Business logic
├── Views/            # HTML templates
├── wwwroot/          # CSS, JS, images
├── Database/         # SQL script
└── appsettings.json  # Configuration
```

---

## Features

- ✅ Multi-role support (Admin, Teacher, Student)
- ✅ Attendance marking (Present/Absent/Late)
- ✅ Real-time analytics
- ✅ Flexible login (Username, Email, Student Code)
- ✅ Activity logging
- ✅ Report export (PDF, Excel)

---

## User Roles

| Role | Capabilities |
|------|--------------|
| **Admin** | Manage courses, students, teachers, enrollments |
| **Teacher** | Mark attendance, view records, export reports |
| **Student** | View personal attendance, check alerts |

---

## Database Tables

- Users (authentication)
- Students
- Teachers
- Courses
- CourseAssignments
- Enrollments
- AttendanceRecords
- ActivityLogs

---

## Technology

- ASP.NET Core MVC 8.0
- SQL Server
- Entity Framework Core
- BCrypt (password hashing)

---

## Configuration

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Server=YOUR_SERVER;Database=SmartAttendanceDB;Trusted_Connection=True;..."
  },
  "AppSettings": {
    "MinAttendanceThreshold": 75.0
  }
}
```

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Database not found | Execute `Database/SmartAttendanceDB.sql` in SSMS |
| Connection error | Check connection string matches your SQL Server |
| Login fails | Verify admin user exists in database |
| Port in use | Change port in `Properties/launchSettings.json` |

---

## Support

For issues: Check error logs in `bin/Debug/` or contact the development team.

---

**Version:** 1.0 | **Framework:** .NET 8.0 | **Database:** SQL Server
