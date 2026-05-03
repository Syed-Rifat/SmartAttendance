using System.Threading.Tasks;
using SmartAttendance.Patterns.Factory;
using SmartAttendance.Repositories.Interfaces;
using SmartAttendance.Services.Interfaces;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System;

namespace SmartAttendance.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _uow;

        public ReportService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<byte[]> ExportAttendanceReportAsync(int assignmentId, string format)
        {
            var assignment = await _uow.CourseAssignments.GetByIdAsync(assignmentId);
            if (assignment == null) return Array.Empty<byte>();

            var course = await _uow.Courses.GetByIdAsync(assignment.CourseId);
            var enrollments = (await _uow.Enrollments.FindAsync(e => e.AssignmentId == assignmentId)).ToList();
            var records = (await _uow.AttendanceRecords.FindAsync(a => a.AssignmentId == assignmentId)).ToList();

            var uniqueDates = records.Select(r => r.AttendanceDate).Distinct().OrderBy(d => d).ToList();

            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Name");
            foreach (var date in uniqueDates)
            {
                dt.Columns.Add(date.ToString("dd-MM-yyyy")); // Format as requested or standard
            }
            dt.Columns.Add("Total Percentage");

            foreach (var e in enrollments)
            {
                var student = await _uow.Students.GetByIdAsync(e.StudentId);
                DataRow row = dt.NewRow();
                row["ID"] = student?.StudentCode ?? "";
                row["Name"] = student?.FullName ?? "";

                int attendedCount = 0;
                int totalDays = uniqueDates.Count;

                foreach (var date in uniqueDates)
                {
                    var record = records.FirstOrDefault(r => r.EnrollmentId == e.EnrollmentId && r.AttendanceDate == date);
                    string status = record?.Status ?? "Absent";
                    row[date.ToString("dd-MM-yyyy")] = status;

                    if (status == "Present" || status == "Late") attendedCount++;
                }

                decimal percentage = totalDays > 0 ? Math.Round((decimal)attendedCount / totalDays * 100, 1) : 0;
                row["Total Percentage"] = percentage + "%";

                dt.Rows.Add(row);
            }

            var exporter = ReportExporterFactory.Create(format);
            return exporter.Export(dt, $"Attendance Report - {course?.CourseName} ({assignment.Section})");
        }
    }
}
