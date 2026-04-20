# Smart Attendance System

[![.NET Core](https://img.shields.io/badge/.NET-8.0-blueviolet)](https://dotnet.microsoft.com)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2019%2B-CC2927)](https://www.microsoft.com/en-us/sql-server/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Build Status](https://img.shields.io/badge/Build-Passing-brightgreen)](https://github.com)

## Overview

**Smart Attendance System** is an enterprise-grade, role-based attendance management solution built with ASP.NET Core MVC. It provides comprehensive tools for educational institutions to efficiently manage course enrollment, track student attendance, and generate analytics dashboards. The system supports three distinct user roles—**Admin**, **Teacher**, and **Student**—each with dedicated features and workflows.

### Key Capabilities

| Capability | Description |
|------------|------------|
| **Multi-Role Architecture** | Independent dashboards and permissions for Admin, Teacher, and Student roles |
| **Flexible Authentication** | Login via Username, Email, or Student Code with secure BCrypt password hashing |
| **Intelligent Tracking** | Real-time attendance marking with present/absent/late status options |
| **Advanced Analytics** | Automated attendance percentage calculations with low-threshold alerts |
| **Audit Trail** | Complete activity logging for compliance and system monitoring |

---

## Table of Contents

- [System Architecture](#system-architecture)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Database Configuration](#database-configuration)
  - [Running the Application](#running-the-application)
- [Authentication & Authorization](#authentication--authorization)
- [User Roles & Capabilities](#user-roles--capabilities)
- [Technology Stack](#technology-stack)
- [Database Design](#database-design)
- [Troubleshooting Guide](#troubleshooting-guide)
- [Configuration](#configuration)

---

## System Architecture

This solution implements a layered architecture pattern with clear separation of concerns:

```
Presentation Layer (Views/Controllers)
         ↓
Business Logic Layer (Services)
         ↓
Data Access Layer (Repositories/UnitOfWork)
         ↓
Data Layer (Entity Framework Core → SQL Server)
```

### Project Directory Structure

│   ├── Views/             # Razor view templates for the UI
│   └── wwwroot/           # Static files (CSS, JS, images, libraries)
```

---

## Getting Started

### Prerequisites

Before proceeding with installation, ensure you have the following prerequisites installed:

| Component | Requirement | Link |
|-----------|------------|------|
| **.NET SDK** | .NET 8.0 or later | [Download](https://dotnet.microsoft.com/download) |
| **SQL Server** | SQL Server 2019 or Express Edition | [Download](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) |
| **SQL Management Tool** | SSMS or Azure Data Studio | [SSMS](https://learn.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms) |
| **IDE** | Visual Studio 2022 or VS Code | [Visual Studio](https://visualstudio.microsoft.com/vs/) |

### Installation

#### Step 1: Clone Repository
**Note:** The project includes a complete pre-configured SQL script. **Do not run EF Core migrations** (`dotnet ef database update`).

**Procedure:**

1. Open **SQL Server Management Studio (SSMS)** or **Azure Data Studio**
2. Connect to your SQL Server instance (e.g., `localhost\SQLEXPRESS`)
3. Open and execute `Database/SmartAttendanceDB.sql`
   
   This script will automatically:
   - ✓ Create `SmartAttendanceDB` database
   - ✓ Create all normalized tables with FK constraints
   - ✓ Deploy database views for analytics
   - ✓ Initialize default Admin user

4. **Update Connection String** in `appsettings.json`:

   ```json
   "ConnectionStrings": {
     "Default": "Server=localhost\\SQLEXPRESS;Database=SmartAttendanceDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
   }
   ```
   
   > Adjust `Server` value to match your SQL Server instance name.

#### Step 3: Running the Application

Once the database is initialized, launch the application using one of these methods:

**Option A: Using Visual Studio**
```
1. Open SmartAttendance.sln
2. Press F5 or Ctrl+F5
3. Application will launch at https://localhost:<port>
```

**Option B: Using Command Line**
```bash
dotnet build
dotnet run
```

---

## Authentication & Authorization

### Default Admin Credentials

Upon successful database initialization, the following admin account is created:

| Field | Value |
|:------|:------|
| **Username** | `admin` |
| **Email** | `admin@attendance.edu` |
| **Password** | `Admin@123` |
| **Role** | Administrator |

### Login Methods

Users can authenticate using any of the following identifiers:

- **Username:** `admin`
- **Email Address:** `admin@attendance.edu`  
- **Student Code** (Students only): e.g., `CSE-2021-042`

---

## User Roles & Capabilities

### 🔐 Administrator Dashboard

Administrators have complete system control with comprehensive management capabilities:

| Function | Capabilities |
|----------|--------------|
| **Course Management** | Create, update, and delete courses with credit hours allocation |
| **User Management** | Manage teachers and students, assign roles, enable/disable accounts |
| **Enrollment** | Enroll students in courses, bulk upload functionality |
| **Assignments** | Assign teachers to courses and sections |
| **Monitoring** | View activity logs, system health metrics, user audit trail |
| **Configuration** | Manage system settings and attendance thresholds |

### 👨‍🏫 Teacher Dashboard

Teachers have access to attendance marking and reporting features:

| Function | Capabilities |
|----------|--------------|
| **Course Overview** | View assigned courses and student enrollments |
| **Attendance Marking** | Mark daily attendance with status options (Present/Absent/Late) |
| **Attendance Reports** | View and export student attendance records |
| **Analytics** | Monitor class attendance patterns and identify at-risk students |
| **Low Attendance Alert** | Automatic identification of students below threshold |

### 👨‍🎓 Student Dashboard

Students have self-service access to their attendance information:

| Function | Capabilities |
|----------|--------------|
| **Attendance View** | Monitor personal attendance records per course |
| **Percentage Tracking** | View calculated attendance percentage |
| **Alert System** | Receive notifications for below-threshold attendance |
| **Report Export** | Download attendance reports for personal records |

---

### 7. Core Features

#### **Intelligent Attendance Tracking**
- Flexible status options: Present, Absent, Late
- Per-student, per-course tracking with date validation
- Automatic percentage calculations using database views
- Real-time low attendance alerts (threshold-based)

#### **Enterprise-Grade Security**
- BCrypt password hashing with salt
- Role-based access control (RBAC)
- Secure session management with cookies
- Complete activity audit logging

#### **Advanced Reporting**
- PDF and Excel export capabilities
- Bulk operation support for data import
- Customizable attendance reports
- Analytics dashboard with visual charts

---

## Technology Stack

### Backend Architecture

| Layer | Technology | Purpose |
|-------|-----------|---------|
| **Presentation** | ASP.NET Core MVC | Web framework and request handling |
| **Business Logic** | Service Layer | Encapsulation of business rules |
| **Data Access** | Repository Pattern, Unit of Work | ORM abstraction and transaction management |
| **Database** | Entity Framework Core 8.0 | ORM for database operations |

### Technology Details

```
┌─────────────────────────────────────────────┐
│   Framework: ASP.NET Core MVC (.NET 8.0)    │
├─────────────────────────────────────────────┤
│   Database: SQL Server 2019+                │
│   ORM: Entity Framework Core                │
│   Authentication: BCrypt.Net-Next           │
│   Architecture Pattern: Repository + SOLID  │
│   Frontend: HTML5, CSS3, JavaScript, Razor  │
│   Session Management: ASP.NET Core Identity │
└─────────────────────────────────────────────┘
```

```

---

## Database Design

### Entity Relationship Diagram (Logical)

```
Users (1) ──── (1) Students
Users (1) ──── (1) Teachers
Teachers (1) ──── (M) CourseAssignments
Courses (1) ──── (M) CourseAssignments
CourseAssignments (1) ──── (M) Enrollments
CourseAssignments (1) ──── (M) AttendanceRecords
Students (1) ──── (M) Enrollments
Enrollments (1) ──── (M) AttendanceRecords
Users (1) ──── (M) ActivityLogs
```

### Data Tables Overview

| Table | Purpose | Key Relationships |
|-------|---------|-------------------|
| **Users** | Authentication & authorization | PK: UserId |
| **Students** | Student information | FK: UserId |
| **Teachers** | Teacher information | FK: UserId |
| **Courses** | Course catalog | PK: CourseId |
| **CourseAssignments** | Teacher-Course-Section mapping | FK: TeacherId, CourseId |
| **Enrollments** | Student-Course registration | FK: StudentId, AssignmentId |
| **AttendanceRecords** | Daily attendance tracking | FK: EnrollmentId, AssignmentId |
| **ActivityLogs** | System audit trail | FK: UserId |

**Performance Optimizations:**
- Indexed columns: Department, Batch, Enrollment, Assignment, AttendanceDate
- View-based calculations for attendance percentages
- Stored procedures for complex queries

---

## Troubleshooting Guide

### Authentication Issues

| Issue | Probable Cause | Solution |
|-------|---|---|
| "Invalid Username or Password" | User not found or wrong credentials | Verify database is initialized; check user exists in Users table |
| "Access Denied" (post-login) | Role mismatch or permission issue | Ensure Role in Users table matches selected login role |
| "Account is inactive" | User IsActive flag set to 0 | Re-enable user via Admin panel or database |

### Database Connectivity

| Issue | Probable Cause | Solution |
|-------|---|---|
| Connection timeout | SQL Server not running | Start SQL Server service: `sqlservermanager` |
| "Cannot find server" | Invalid server name in connection string | Verify server name matches `(local)\SQLEXPRESS` or your instance |
| Permission denied | Trusted connection not enabled | Enable "Trusted_Connection=True" in connection string |
| Database not found | SQL script not executed | Execute `Database/SmartAttendanceDB.sql` in SSMS |

### Application Launch Issues

| Issue | Solution |
|-------|----------|
| Build fails with dependency errors | Run `dotnet restore` to download NuGet packages |
| Port already in use | Change port in `Properties/launchSettings.json` |
| HTTPS certificate error | Use `dotnet dev-certs https --trust` |

**Diagnostic Command:**
```bash
# Full rebuild with verbose output
dotnet clean
dotnet restore
dotnet build --verbosity detailed
dotnet run --verbosity information
```

---

## Configuration

### Application Settings (`appsettings.json`)

**Connection String Configuration:**
```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost\\SQLEXPRESS;Database=SmartAttendanceDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "AppSettings": {
    "MinAttendanceThreshold": 75.0
  }
}
```

### Key Configuration Parameters

| Parameter | Default | Description |
|-----------|---------|-------------|
| `MinAttendanceThreshold` | 75.0 | Attendance percentage below which students trigger alerts |
| `Trusted_Connection` | True | Use Windows authentication (disable for SQL Server auth) |
| `MultipleActiveResultSets` | true | Allow multiple concurrent operations on connection |
| `TrustServerCertificate` | True | Accept self-signed SSL certificates |

**Example: Changing Attendance Threshold**
```json
"AppSettings": {
  "MinAttendanceThreshold": 80.0  // Changes threshold to 80%
}
```

---

## Development Workflow

The recommended workflow for different user types:

```
┌─────────────────────────────────────────────────────────┐
│  ADMIN WORKFLOW                                         │
├─────────────────────────────────────────────────────────┤
│ 1. Login with admin credentials                         │
│ 2. Create courses in the system                         │
│ 3. Add teachers and students (bulk or individual)       │
│ 4. Assign teachers to courses and sections              │
│ 5. Enroll students in courses                           │
│ 6. Monitor activity logs and system health              │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│  TEACHER WORKFLOW                                       │
├─────────────────────────────────────────────────────────┤
│ 1. Login with teacher credentials                       │
│ 2. View assigned courses and enrolled students          │
│ 3. Mark attendance for each class session               │
│ 4. Monitor student attendance records                   │
│ 5. Export attendance reports for records                │
│ 6. Identify and flag at-risk students                   │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│  STUDENT WORKFLOW                                       │
├─────────────────────────────────────────────────────────┤
│ 1. Login using student code or email                    │
│ 2. View personal attendance dashboard                   │
│ 3. Check attendance % per enrolled course               │
│ 4. Receive alerts for below-threshold attendance        │
│ 5. Export attendance records when needed                │
└─────────────────────────────────────────────────────────┘
```

---

## Security Considerations

### Best Practices Implemented

✅ **Password Security**
- BCrypt hashing with salt
- No plaintext password storage
- Configurable hash rounds for future updates

✅ **Access Control**
- Role-based authorization at controller level
- Claims-based identity management
- Session management via secure cookies

✅ **Audit & Compliance**
- Complete activity logging
- User action tracking with timestamps
- Accountability trail for attendance changes

### Recommended Production Steps

1. Change default admin password immediately after deployment
2. Enable HTTPS only (configure SSL certificate)
3. Set up database backups and recovery procedures
4. Implement IP whitelisting if necessary
5. Monitor activity logs regularly
6. Use strong SQL Server authentication in production

---

## Performance & Scalability

### Database Optimization

- **Indexes:** Implemented on frequently queried columns (Department, StudentId, AssignmentId, AttendanceDate)
- **Views:** Pre-calculated attendance summaries reduce query complexity
- **Normalization:** 3NF schema design prevents data redundancy
- **Connection Pooling:** MARS enabled for concurrent operations

### Scalability Recommendations

For institutions with 5,000+ students:
- Implement database archiving for historical records
- Consider SQL Server Enterprise Edition
- Set up read replicas for reporting queries
- Implement caching for dashboard data

---

## Support & Maintenance

### Getting Help

If you encounter issues:

1. **Check Logs:** Review application logs in `bin/Debug/` or `bin/Release/`
2. **Database Verification:** Validate schema and data integrity via SSMS
3. **Documentation:** Refer to code comments in Controllers and Services
4. **Community:** Report issues or seek help through project repository

### Regular Maintenance Tasks

| Task | Frequency | Purpose |
|------|-----------|---------|
| Database Backup | Daily | Disaster recovery |
| Activity Log Cleanup | Monthly | Storage optimization |
| Password Updates | Quarterly | Security refresh |
| Dependency Updates | Quarterly | Security patches |

---

## License & Attribution

This project is provided as-is for educational and institutional use. For licensing details, see LICENSE file.

**Built with:** .NET 8.0 | SQL Server | Entity Framework Core | ASP.NET Core MVC

---

**Last Updated:** April 2026 | Version 1.0
