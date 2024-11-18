using Entities.MasterData;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

        public static ExcelReadResponseDto ReadExcelFile(string filePath, string userid, string query, string excelCol, string tableName, string title, ArrayList conditions = null, ArrayList condRemark = null, string excelRange = "A4:BU5000", string uniqueField = "", ArrayList specialCond = null, string addCond = "", ArrayList superconditions = null, ArrayList supercondRemark = null)
        {
            var dataTable = new DataTable();
            string extension = Path.GetExtension(filePath).ToLower();

            if (extension == ".txt")
            {
                // Handle .txt file
                using (var sr = new StreamReader(filePath, Encoding.UTF8))
                {
                    string line;
                    bool isFirstLine = true;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var fields = line.Split(',');
                        if (isFirstLine)
                        {
                            // Create columns
                            foreach (var field in fields)
                            {
                                dataTable.Columns.Add(field.Trim());
                            }
                            isFirstLine = false;
                        }
                        else
                        {
                            // Add data rows
                            var row = dataTable.NewRow();
                            for (int i = 0; i < fields.Length; i++)
                            {
                                row[i] = fields[i].Trim();
                            }
                            dataTable.Rows.Add(row);
                        }
                    }
                }
            }
            else
            {
                // Handle Excel file
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();

                    if (worksheet == null)
                    {
                        return new ExcelReadResponseDto { Message = "No worksheet found in the Excel file." };
                    }

                    var startRow = 4;

                    var expectedColumns = excelCol
                                           .Split(',')
                                           .Select(col => col.Replace("[", "").Replace("]", "").Trim())
                                           .ToArray();

                    // Validate the header
                    for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                    {
                        var columnHeader = worksheet.Cells[startRow, col].Text.Trim();
                        if (string.IsNullOrWhiteSpace(columnHeader))
                        {
                            return new ExcelReadResponseDto { Message = "Invalid Data structure, please follow template format." };
                        }
                        else
                        {
                            dataTable.Columns.Add(columnHeader);
                        }
                    }

                    // Check if all expected columns are present
                    foreach (var expectedColumn in expectedColumns)
                    {
                        if (!dataTable.Columns.Contains(expectedColumn))
                        {
                            return new ExcelReadResponseDto { Message = "Invalid Data structure, please follow template format." };
                        }
                    }

                    // Read data from Excel into DataTable
                    for (int row = startRow + 1; row <= worksheet.Dimension.End.Row; row++)
                    {
                        var rowData = dataTable.NewRow();
                        for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                        {
                            rowData[col - 1] = worksheet.Cells[row, col].Text.Trim();
                        }
                        dataTable.Rows.Add(rowData);
                    }
                }
            }

            return new ExcelReadResponseDto { DataTable = dataTable };
        }
    }
}
    