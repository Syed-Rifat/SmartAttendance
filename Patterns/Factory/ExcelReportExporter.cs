using System.Collections.Generic;
using ClosedXML.Excel;
using System.IO;

namespace SmartAttendance.Patterns.Factory
{
    public class ExcelReportExporter : IReportExporter
    {
        public byte[] Export<T>(IEnumerable<T> data, string title)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Report");
                
                // Add Title
                worksheet.Cell(1, 1).Value = title;
                worksheet.Range(1, 1, 1, 5).Merge().Style.Font.SetBold().Font.FontSize = 14;

                // Reflection could be used here to write headers and rows based on type T
                // For simplicity, we just put a placeholder if T is complex.
                // In a real app, you would iterate over properties.
                
                var properties = typeof(T).GetProperties();
                int col = 1;
                foreach(var prop in properties)
                {
                    worksheet.Cell(2, col).Value = prop.Name;
                    worksheet.Cell(2, col).Style.Font.SetBold();
                    col++;
                }

                int row = 3;
                foreach(var item in data)
                {
                    col = 1;
                    foreach(var prop in properties)
                    {
                        var val = prop.GetValue(item, null);
                        worksheet.Cell(row, col).Value = val?.ToString() ?? "";
                        col++;
                    }
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}
