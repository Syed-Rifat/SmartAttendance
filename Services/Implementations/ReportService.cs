using System.Threading.Tasks;
using SmartAttendance.Patterns.Factory;
using SmartAttendance.Repositories.Interfaces;
using SmartAttendance.Services.Interfaces;
using System.Linq;

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
            var records = await _uow.AttendanceRecords.FindAsync(a => a.AssignmentId == assignmentId);
            // Fetch related data properly in a real scenario (e.g. Include(Student))
            // Here we just project to a simple anonymous or DTO object for the exporter
            var dataToExport = records.Select(r => new {
                r.AttendanceDate,
                r.EnrollmentId,
                r.Status
            });

            var exporter = ReportExporterFactory.Create(format);
            return exporter.Export(dataToExport, $"Attendance Report - Assignment {assignmentId}");
        }
    }
}
