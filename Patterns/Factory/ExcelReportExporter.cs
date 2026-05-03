using System;
using System.Collections.Generic;
using ClosedXML.Excel;
using System.IO;
using System.Data;

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

        public byte[] Export(DataTable data, string title)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Attendance Report");

                // Add Title
                worksheet.Cell(1, 1).Value = title;
                worksheet.Range(1, 1, 1, Math.Max(data.Columns.Count, 5)).Merge().Style.Font.SetBold().Font.FontSize = 14;

                // Headers
                worksheet.Row(2).Height = 80; // Set height for vertical dates
                for (int i = 0; i < data.Columns.Count; i++)
                {
                    var cell = worksheet.Cell(2, i + 1);
                    cell.Value = data.Columns[i].ColumnName;
                    cell.Style.Font.SetBold();
                    cell.Style.Fill.BackgroundColor = XLColor.LightGray;
                    cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    // If it's a date-like header (after ID and Name, and before Total Percentage), rotate it
                    if (i >= 2 && i < data.Columns.Count - 1)
                    {
                        cell.Style.Alignment.TextRotation = 90;
                    }
                }

                // Data Rows
                for (int r = 0; r < data.Rows.Count; r++)
                {
                    for (int c = 0; c < data.Columns.Count; c++)
                    {
                        var cell = worksheet.Cell(r + 3, c + 1);
                        var val = data.Rows[r][c];
                        cell.Value = val?.ToString() ?? "";

                        // Styling for Present/Absent
                        if (c >= 2 && c < data.Columns.Count - 1)
                        {
                            string status = val?.ToString() ?? "";
                            if (status == "Present") cell.Style.Font.FontColor = XLColor.Green;
                            else if (status == "Absent") cell.Style.Font.FontColor = XLColor.Red;
                        }
                    }
                }

                worksheet.Columns().AdjustToContents();
                // Ensure the date columns are not too wide after rotation
                for (int i = 2; i < data.Columns.Count - 1; i++)
                {
                    worksheet.Column(i + 1).Width = 5;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}
