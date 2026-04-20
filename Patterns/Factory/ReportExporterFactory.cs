using System;

namespace SmartAttendance.Patterns.Factory
{
    public class ReportExporterFactory
    {
        public static IReportExporter Create(string type)
        {
            if (type.Equals("excel", StringComparison.OrdinalIgnoreCase))
            {
                return new ExcelReportExporter();
            }
            else if (type.Equals("pdf", StringComparison.OrdinalIgnoreCase))
            {
                return new PdfReportExporter();
            }
            
            throw new ArgumentException("Invalid exporter type.");
        }
    }
}
