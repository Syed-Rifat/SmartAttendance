using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartAttendance.Services.Interfaces
{
    public interface IReportService
    {
        Task<byte[]> ExportAttendanceReportAsync(int assignmentId, string format);
    }
}
