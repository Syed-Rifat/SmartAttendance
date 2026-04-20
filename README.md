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
├── Database/          # Contains the SQL script for database creation
├── Models/            # Domain entities and ViewModels
├── Repositories/      # Data access layer abstracting Entity Framework
├── Services/          # Business logic layer
├── Patterns/          # Design patterns implementations (Factory, Singleton, etc.)
├── Filters/           # Action filters (e.g., ActivityLogFilter)
├── Views/             # Razor view templates for the UI
└── wwwroot/           # Static files (CSS, JS, images, libraries)
```

## Step-by-Step Setup Instructions

Follow these steps in sequence to set up and run the project locally.

### 1. Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express edition is sufficient)
- SQL Server Management Studio (SSMS) or Azure Data Studio
- Visual Studio 2022 or Visual Studio Code

### 2. Clone the Repository
Open your terminal or command prompt and clone the project:
```bash
git clone https://github.com/Syed-Rifat/SmartAttendance.git
cd SmartAttendance
```

### 3. Database Setup (Crucial Step)

For this project, we have provided a complete raw SQL script that contains the database schema, tables, views, and stored procedures. **You do not need to run EF Core migrations (`dotnet ef database update`).** Instead, follow these steps:

1. Open **SQL Server Management Studio (SSMS)** or Azure Data Studio and connect to your local SQL Server instance (e.g., `localhost\SQLEXPRESS`).
2. Open the file `Database/SmartAttendanceDB.sql` provided in the repository.
3. Execute the script. This will automatically:
   - Create the `SmartAttendanceDB` database.
   - Create all necessary tables with relationships (Users, Students, Teachers, Courses, etc.).
   - Create database Views and Stored Procedures for attendance calculations.
   - Insert a default Admin user.
4. **Update Connection String:** Open `appsettings.json` in the project root and ensure the connection string matches your SQL Server instance name:
   ```json
   "ConnectionStrings": {
     "Default": "Server=YOUR_SERVER_NAME;Database=SmartAttendanceDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
   }
   ```
   *(If you are using default SQL Express, `Server=localhost\\SQLEXPRESS` will work fine).*

### 4. Running the Application
Once the database is set up, you can run the application:

**Using Visual Studio:**
- Open `SmartAttendance.sln`.
- Press `F5` or `Ctrl+F5` to run the project.

**Using Command Line:**
```bash
dotnet build
dotnet run
```
The application will be hosted at `https://localhost:<port>` or `http://localhost:<port>`.

### 5. Initial Login Credentials
A default Admin account is created by the SQL script. You will need to replace the placeholder password hash in the database, or use the registration flow if available.
- **Username:** `admin`
- **Email:** `admin@attendance.edu`
- **Role:** `Admin`

## Technology Stack
- **Framework**: ASP.NET Core MVC
- **Database**: SQL Server
- **ORM**: Entity Framework Core
- **Frontend**: HTML5, CSS3, JavaScript, Razor Pages
