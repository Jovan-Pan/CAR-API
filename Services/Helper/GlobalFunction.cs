using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helper
{
    public static class GlobalFunction
    {
        public static string GetContentType(string fileExtension)
        {
            return fileExtension switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream",
            };
        }

        public static DataTable ReadExcelFile(string filePath)
        {
            var dataTable = new DataTable();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified file was not found.", filePath);
            }

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null || worksheet.Dimension == null)
                {
                    throw new Exception("The worksheet is either blank or invalid.");
                }

                var startRow = 4;
                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                {
                    var columnHeader = worksheet.Cells[startRow, col].Text.Trim();

                    if (string.IsNullOrWhiteSpace(columnHeader))
                    {
                        columnHeader = $"Column{col}";
                    }

                    if (!dataTable.Columns.Contains(columnHeader))
                    {
                        dataTable.Columns.Add(columnHeader);
                    }
                }

                for (int row = startRow + 1; row <= worksheet.Dimension.End.Row; row++)
                {
                    var rowData = dataTable.NewRow();
                    for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                    {
                        rowData[col - 1] = worksheet.Cells[row, col].Text;
                    }
                    dataTable.Rows.Add(rowData);
                }
            }

            return dataTable;
        }

    }
}
