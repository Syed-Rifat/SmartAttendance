-- ============================================================
-- Smart Attendance Management System
-- Database: SQL Server
-- Standard: 3NF Normalized, FK Constraints, Indexes
-- ============================================================

CREATE DATABASE SmartAttendanceDB;
GO
USE SmartAttendanceDB;
GO

-- ============================================================
-- TABLE 1: Users (Authentication & Role)
-- ============================================================
CREATE TABLE Users (
    UserId       INT IDENTITY(1,1) PRIMARY KEY,
    Username     NVARCHAR(50)  NOT NULL UNIQUE,
    Email        NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL,
    Role         NVARCHAR(20)  NOT NULL CHECK (Role IN ('Admin','Teacher','Student')),
    IsActive     BIT           NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2     NOT NULL DEFAULT GETDATE()
);
GO

-- ============================================================
-- TABLE 2: Students
-- ============================================================
CREATE TABLE Students (
    StudentId     INT IDENTITY(1,1) PRIMARY KEY,
    UserId        INT           NOT NULL UNIQUE REFERENCES Users(UserId) ON DELETE CASCADE,
    StudentCode   NVARCHAR(20)  NOT NULL UNIQUE,   -- e.g. "CSE-2021-042"
    FullName      NVARCHAR(100) NOT NULL,
    Department    NVARCHAR(60)  NOT NULL,
    Batch         NVARCHAR(10)  NOT NULL,
    Section       NVARCHAR(5)   NOT NULL,
    ContactNumber NVARCHAR(15)  NULL
);
GO

-- ============================================================
-- TABLE 3: Teachers
-- ============================================================
CREATE TABLE Teachers (
    TeacherId     INT IDENTITY(1,1) PRIMARY KEY,
    UserId        INT           NOT NULL UNIQUE REFERENCES Users(UserId) ON DELETE CASCADE,
    FullName      NVARCHAR(100) NOT NULL,
    Department    NVARCHAR(60)  NOT NULL,
    ContactNumber NVARCHAR(15)  NULL
);
GO

-- ============================================================
-- TABLE 4: Courses
-- ============================================================
CREATE TABLE Courses (
    CourseId    INT IDENTITY(1,1) PRIMARY KEY,
    CourseCode  NVARCHAR(20)  NOT NULL UNIQUE,
    CourseName  NVARCHAR(100) NOT NULL,
    Department  NVARCHAR(60)  NOT NULL,
    CreditHours INT           NOT NULL CHECK (CreditHours > 0),
    CreatedAt   DATETIME2     NOT NULL DEFAULT GETDATE()
);
GO

-- ============================================================
-- TABLE 5: CourseAssignments (Teacher teaches a Course in a Section)
-- ============================================================
CREATE TABLE CourseAssignments (
    AssignmentId  INT IDENTITY(1,1) PRIMARY KEY,
    TeacherId     INT          NOT NULL REFERENCES Teachers(TeacherId),
    CourseId      INT          NOT NULL REFERENCES Courses(CourseId),
    Section       NVARCHAR(5)  NOT NULL,
    Semester      NVARCHAR(20) NOT NULL,   -- e.g. "Spring 2025"
    AcademicYear  NVARCHAR(10) NOT NULL,   -- e.g. "2024-25"
    Room          NVARCHAR(20) NULL,
    Schedule      NVARCHAR(100) NULL,      -- e.g. "Mon/Wed 10:00-11:30"
    CONSTRAINT UQ_Assignment UNIQUE (TeacherId, CourseId, Section, Semester, AcademicYear)
);
GO

-- ============================================================
-- TABLE 6: Enrollments (Student <-> CourseAssignment)
-- ============================================================
CREATE TABLE Enrollments (
    EnrollmentId INT IDENTITY(1,1) PRIMARY KEY,
    StudentId    INT       NOT NULL REFERENCES Students(StudentId),
    AssignmentId INT       NOT NULL REFERENCES CourseAssignments(AssignmentId),
    EnrolledAt   DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_Enrollment UNIQUE (StudentId, AssignmentId)
);
GO

-- ============================================================
-- TABLE 7: AttendanceRecords
-- ============================================================
CREATE TABLE AttendanceRecords (
    AttendanceId        INT IDENTITY(1,1) PRIMARY KEY,
    EnrollmentId        INT          NOT NULL REFERENCES Enrollments(EnrollmentId),
    AssignmentId        INT          NOT NULL REFERENCES CourseAssignments(AssignmentId),
    AttendanceDate      DATE         NOT NULL,
    Status              NVARCHAR(10) NOT NULL CHECK (Status IN ('Present','Absent','Late')),
    MarkedAt            DATETIME2    NOT NULL DEFAULT GETDATE(),
    MarkedByTeacherId   INT          NOT NULL REFERENCES Teachers(TeacherId),
    EditedAt            DATETIME2    NULL,
    CONSTRAINT UQ_Attendance UNIQUE (EnrollmentId, AttendanceDate)
);
GO

-- ============================================================
-- TABLE 8: ActivityLogs
-- ============================================================
CREATE TABLE ActivityLogs (
    LogId       INT IDENTITY(1,1) PRIMARY KEY,
    UserId      INT            NOT NULL REFERENCES Users(UserId),
    Action      NVARCHAR(50)   NOT NULL,   -- e.g. "TakeAttendance", "Login"
    Description NVARCHAR(255)  NULL,
    Timestamp   DATETIME2      NOT NULL DEFAULT GETDATE()
);
GO

-- ============================================================
-- INDEXES
-- ============================================================
CREATE INDEX IX_Students_Department    ON Students(Department);
CREATE INDEX IX_Students_Batch         ON Students(Batch);
CREATE INDEX IX_Enrollments_StudentId  ON Enrollments(StudentId);
CREATE INDEX IX_Enrollments_AssignId   ON Enrollments(AssignmentId);
CREATE INDEX IX_Attendance_Date        ON AttendanceRecords(AttendanceDate);
CREATE INDEX IX_Attendance_Assignment  ON AttendanceRecords(AssignmentId);
CREATE INDEX IX_ActivityLogs_UserId    ON ActivityLogs(UserId);
GO

-- ============================================================
-- VIEW 1: vw_AttendanceSummary
-- Shows each student's attendance % per course assignment
-- ============================================================
CREATE VIEW vw_AttendanceSummary AS
SELECT
    s.StudentId,
    s.StudentCode,
    s.FullName         AS StudentName,
    c.CourseCode,
    c.CourseName,
    ca.Section,
    ca.Semester,
    COUNT(ar.AttendanceId)                                        AS TotalClasses,
    SUM(CASE WHEN ar.Status IN ('Present','Late') THEN 1 ELSE 0 END) AS ClassesAttended,
    CAST(
        SUM(CASE WHEN ar.Status IN ('Present','Late') THEN 1 ELSE 0 END) * 100.0
        / NULLIF(COUNT(ar.AttendanceId), 0)
    AS DECIMAL(5,2))                                              AS AttendancePercentage
FROM Students s
JOIN Enrollments e     ON s.StudentId    = e.StudentId
JOIN CourseAssignments ca ON e.AssignmentId = ca.AssignmentId
JOIN Courses c         ON ca.CourseId    = c.CourseId
LEFT JOIN AttendanceRecords ar ON e.EnrollmentId = ar.EnrollmentId
GROUP BY
    s.StudentId, s.StudentCode, s.FullName,
    c.CourseCode, c.CourseName,
    ca.Section, ca.Semester;
GO

-- ============================================================
-- VIEW 2: vw_DailyAttendanceSummary
-- Admin dashboard: per-course daily totals
-- ============================================================
CREATE VIEW vw_DailyAttendanceSummary AS
SELECT
    ar.AttendanceDate,
    c.CourseCode,
    c.CourseName,
    ca.Section,
    t.FullName   AS TeacherName,
    COUNT(*)                                                          AS TotalStudents,
    SUM(CASE WHEN ar.Status = 'Present' THEN 1 ELSE 0 END)           AS PresentCount,
    SUM(CASE WHEN ar.Status = 'Absent'  THEN 1 ELSE 0 END)           AS AbsentCount,
    SUM(CASE WHEN ar.Status = 'Late'    THEN 1 ELSE 0 END)           AS LateCount
FROM AttendanceRecords ar
JOIN CourseAssignments ca ON ar.AssignmentId   = ca.AssignmentId
JOIN Courses c            ON ca.CourseId       = c.CourseId
JOIN Teachers t           ON ca.TeacherId      = t.TeacherId
GROUP BY ar.AttendanceDate, c.CourseCode, c.CourseName, ca.Section, t.FullName;
GO

-- ============================================================
-- STORED PROCEDURE 1: sp_TakeAttendance
-- Insert or update attendance for a list of students on a date
-- ============================================================
CREATE PROCEDURE sp_TakeAttendance
    @AssignmentId      INT,
    @AttendanceDate    DATE,
    @MarkedByTeacherId INT,
    @EnrollmentId      INT,
    @Status            NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;

    -- Precondition: Status must be valid
    IF @Status NOT IN ('Present','Absent','Late')
    BEGIN
        RAISERROR('Invalid status value. Must be Present, Absent, or Late.', 16, 1);
        RETURN;
    END

    -- Precondition: Future dates not allowed
    IF @AttendanceDate > CAST(GETDATE() AS DATE)
    BEGIN
        RAISERROR('Cannot record attendance for a future date.', 16, 1);
        RETURN;
    END

    IF EXISTS (
        SELECT 1 FROM AttendanceRecords
        WHERE EnrollmentId = @EnrollmentId AND AttendanceDate = @AttendanceDate
    )
    BEGIN
        -- Update existing record (edit within allowed window handled in app layer)
        UPDATE AttendanceRecords
        SET Status   = @Status,
            EditedAt = GETDATE()
        WHERE EnrollmentId  = @EnrollmentId
          AND AttendanceDate = @AttendanceDate;
    END
    ELSE
    BEGIN
        INSERT INTO AttendanceRecords
            (EnrollmentId, AssignmentId, AttendanceDate, Status, MarkedByTeacherId)
        VALUES
            (@EnrollmentId, @AssignmentId, @AttendanceDate, @Status, @MarkedByTeacherId);
    END
END;
GO

-- ============================================================
-- STORED PROCEDURE 2: sp_GetLowAttendanceStudents
-- Returns students below a threshold % for a given assignment
-- ============================================================
CREATE PROCEDURE sp_GetLowAttendanceStudents
    @AssignmentId INT,
    @Threshold    DECIMAL(5,2) = 75.0
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        s.StudentCode,
        s.FullName,
        s.ContactNumber,
        v.AttendancePercentage
    FROM vw_AttendanceSummary v
    JOIN Students s ON v.StudentId = s.StudentId
    JOIN CourseAssignments ca ON ca.Section = v.Section
                             AND ca.Semester = v.Semester
    WHERE ca.AssignmentId      = @AssignmentId
      AND v.AttendancePercentage < @Threshold
    ORDER BY v.AttendancePercentage ASC;
END;
GO

-- ============================================================
-- FUNCTION: fn_GetAttendancePercentage
-- Returns attendance % for one student in one assignment
-- ============================================================
CREATE FUNCTION fn_GetAttendancePercentage
(
    @StudentId    INT,
    @AssignmentId INT
)
RETURNS DECIMAL(5,2)
AS
BEGIN
    DECLARE @Pct DECIMAL(5,2);

    SELECT @Pct = CAST(
        SUM(CASE WHEN ar.Status IN ('Present','Late') THEN 1 ELSE 0 END) * 100.0
        / NULLIF(COUNT(ar.AttendanceId), 0)
    AS DECIMAL(5,2))
    FROM Enrollments e
    JOIN AttendanceRecords ar ON e.EnrollmentId = ar.EnrollmentId
    WHERE e.StudentId    = @StudentId
      AND e.AssignmentId = @AssignmentId;

    RETURN ISNULL(@Pct, 0.00);
END;
GO

-- ============================================================
-- SEED: Default Admin User
-- Password: Admin@123  (bcrypt hash placeholder — replace at runtime)
-- ============================================================
INSERT INTO Users (Username, Email, PasswordHash, Role)
VALUES ('admin', 'admin@attendance.edu',
        '$2a$11$REPLACE_WITH_BCRYPT_HASH', 'Admin');
GO
