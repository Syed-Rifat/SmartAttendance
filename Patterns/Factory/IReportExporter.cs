using System.Collections.Generic;

namespace SmartAttendance.Patterns.Factory
{
    public interface IReportExporter
    {
        byte[] Export<T>(IEnumerable<T> data, string title);
        byte[] Export(System.Data.DataTable data, string title);
    }
}
