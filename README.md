# Smart Attendance System

A robust, role-based Smart Attendance System built with ASP.NET Core MVC. This application allows administrators to manage courses, students, and teachers, enables teachers to track and manage class attendance, and provides students with a dashboard to monitor their own attendance records.

## Features
- **Role-Based Access Control**: Separate dashboards and functionalities for Admin, Teacher, and Student roles.
- **Unified Login**: Login using Username, Email, or an assigned Code.
- **Attendance Tracking**: Efficiently mark and track attendance per class.
- **Dashboard & Analytics**: View attendance percentages, low-attendance warnings, and overall course statistics.

## Project Structure

The solution follows a standard ASP.NET Core MVC architecture with the addition of Repository and Service patterns for clean separation of concerns.

```text
SmartAttendance/
├── Controllers/       # Handles incoming HTTP requests and responses
├── Data/              # Database context (AppDbContext)
├── Models/            # Domain entities and ViewModels
│   ├── Entities/      # Database models (User, Course, Enrollment, etc.)
│   └── ViewModels/    # Data transfer objects for views
├── Repositories/      # Data access layer abstracting Entity Framework
├── Services/          # Business logic layer
├── Patterns/          # Design patterns implementations (Factory, Singleton, etc.)
├── Filters/           # Action filters (e.g., ActivityLogFilter)
├── Views/             # Razor view templates for the UI
└── wwwroot/           # Static files (CSS, JS, images, libraries)
```

## Setup Instructions

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download) (or corresponding .NET SDK version)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express edition is sufficient)
- Visual Studio 2022 or Visual Studio Code

### 1. Clone the Repository
Clone the project to your local machine using git:
```bash
git clone https://github.com/Syed-Rifat/SmartAttendance.git
cd SmartAttendance
```

### 2. Database Setup
The application uses Entity Framework Core with SQL Server. 

1. Ensure your SQL Server instance is running. The default connection string expects a local SQL Express instance `localhost\SQLEXPRESS`.
2. If your SQL Server instance has a different name, update the `ConnectionStrings:Default` in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "Default": "Server=YOUR_SERVER_NAME;Database=SmartAttendanceDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
   }
   ```
3. Open a terminal or Package Manager Console in the project directory.
4. Run Entity Framework Core migrations to create the database and tables:
   ```bash
   dotnet ef database update
   ```
   *Note: If you don't have the EF Core CLI tools installed, install them first by running `dotnet tool install --global dotnet-ef`.*

### 3. Running the Application
You can run the application via Visual Studio by pressing `F5` or `Ctrl+F5`, or via the command line:

```bash
dotnet run
```
The application will start and be available at `https://localhost:<port>` or `http://localhost:<port>`.

### Initial Credentials
*(Add any default admin credentials here if database seeding is implemented)*

## Technology Stack
- **Framework**: ASP.NET Core MVC
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Frontend**: HTML5, CSS3, JavaScript, Razor Pages
