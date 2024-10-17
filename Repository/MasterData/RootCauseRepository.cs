using Azure.Core;
using Contracts.Repository.MasterData;
using Dapper;
using Entities;
using Entities.CAR;
using Entities.MasterData;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Repository.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.Transactions;
using Services.Helper;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Drawing;
using OfficeOpenXml.Style;

namespace Repository.MasterData
{
    internal sealed class RootCauseRepository(DbContext dbContext) : IRootCauseRepository
    {
        public async Task<IEnumerable<string>> getRootCauseList(int plant)
        {
            string query = RootCauseQuery.getRootCauseList;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<string>(query, new { plant = plant });
        }
        public async Task<IEnumerable<RootCauseCategoryDto>> GetRootCauseCategory(string search,string SearchADV)
        {
            string query;

            if (string.IsNullOrEmpty(search) && string.IsNullOrEmpty(SearchADV))
            {
                query = RootCauseQuery.GetRootCauseCategory;
            }
            else if(!string.IsNullOrEmpty(SearchADV))
            {
                query = RootCauseQuery.SearchadvData;
            }
            else
            {
                query = RootCauseQuery.SearchDatainDB;
            }
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<RootCauseCategoryDto>(query, new { search = search, SearchADV = SearchADV });
        }
        public async Task<IEnumerable<RootCauseCategoryDto>> InsertNewRootCauseCategory(string RootCauseName, string userId, int plant)
        {
            string query = RootCauseQuery.InsertNewRootCauseName;

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<RootCauseCategoryDto>(query, new { RootCauseName = RootCauseName , userId = userId , plant = plant });
        }
        public async Task<IEnumerable<RootCauseCategoryDto>> UpdateNewRootCauseCategory(string RootCauseName, int id, string userId)
        {
            string query = RootCauseQuery.UpdateNewRootCauseCategory;

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<RootCauseCategoryDto>(query, new { RootCauseName = RootCauseName,id=id , userId = userId });
        }
        public async Task<IEnumerable<RootCauseCategoryDto>> DataDelete(int id, string userId)
        {
            string query = RootCauseQuery.DataDelete;

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<RootCauseCategoryDto>(query, new { id = id , userId = userId });
        }
        public async Task<IEnumerable<RootCauseCategoryDto>> DataPermDelete(int id)
        {
            string query = RootCauseQuery.DataPermDelete;

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<RootCauseCategoryDto>(query, new { id = id });
        }
        public async Task<IEnumerable<RootCauseCategoryDto>> DataRecover(int id, string userId)
        {
            string query = RootCauseQuery.DataRecover;

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<RootCauseCategoryDto>(query, new { id = id , userId = userId });
        }
        public async Task<byte[]> Template()
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "\\Excel";
            string fileName = string.Format("Root Cause Category_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string fullPath = string.Format("{0}\\{1}", filePath, fileName);

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            using (var excel = new ExcelPackage(new FileInfo(fullPath)))
            {
                var sheet1 = excel.Workbook.Worksheets.Add("Template");

                WriteTemplateContent(sheet1);

                await excel.SaveAsync();
            }

            //write to memory stream
            MemoryStream ms = new();
            using (FileStream file = new(fullPath, FileMode.Open, FileAccess.Read))
            {
                await file.CopyToAsync(ms);
                ms.Position = 0;
            }

            //if (File.Exists(fullPath))
            //    File.Delete(fullPath);
            //return await System.IO.File.ReadAllBytesAsync(fileName);
            return await File.ReadAllBytesAsync(fullPath);
        }

        private static void WriteTemplateContent(ExcelWorksheet sheet)
        {
            var row1Header = new object[] { "(Please Don't Delete Highlighted Row)" };
            var row2Header = new object[] { "Mandatory", "Mandatory" };
            var row3Header = new object[] { "int", "nvarchar(50)" };
            var row4Header = new object[] { "Plant", "Root Cause Name" };
            var row5Header = new object[] { "2100", "Test" };

            var data = new List<object[]>
            {
            //HEADER
            row1Header,
            row2Header,
            row3Header,
            row4Header,
            row5Header
            };


            //write formula
            const int startRow = 1;
            const int startColumn = 1;

            sheet.Cells[startRow, startColumn].LoadFromArrays(data);

            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();

            using (var range = sheet.Cells[1, row1Header.Length])
            {
                range.Style.Font.Color.SetColor(Color.Red); // Set the font color to red
            }
            using (var range = sheet.Cells[1, 1, 3, 2])
            {
                range.Style.Fill.PatternType = ExcelFillStyle.Solid; // Set the fill pattern
                range.Style.Fill.BackgroundColor.SetColor(Color.LightBlue); // Set the background color
            }

            for (int row = 0; row < data.Count; row++)
            {
                for (int col = 0; col < row2Header.Length; col++)
                {
                    if (col == row2Header.Length - 1 && row >= 5)
                    {
                        sheet.Cells[row + startRow, col + startColumn].Formula = (string)data[row][col];
                    }
                }
            }

            //set date format for actual cr start
            const int startFromRow = 1; // Skip the first two rows as headers
            int endRow = data.Count; // Last row in the worksheet
            const int columnNumber = 1; // Column D
            var columnRange = sheet.Cells[startFromRow, columnNumber, endRow, columnNumber];

            columnRange.Style.Numberformat.Format = "mm/dd/yyyy";
        }

        public async Task<IEnumerable<string>> Import(string filePath, string userId)
        {
            var userid = userId;

            if (filePath == null || filePath.Length == 0)
            {
                return new List<string> { "Invalid Data structure, please follow template format" };
            }
      
            string excelCol = "[Plant], [Root Cause Name]";
            string excelRange = "A4:E5000";
            string query = RootCauseQuery.Import;
            ArrayList conditions = new ArrayList(); 
            ArrayList condRemark = new ArrayList(); 
            ArrayList specialCond = new ArrayList(); 
            conditions.Add(" ISNULL([Root Cause Name], '') = ''  or ISNULL(Plant, '') = ''  ");
            condRemark.Add("Null Mandatory Data");
            conditions.Add(" LEN([Root Cause Name]) > 50");
            condRemark.Add("Process Group Code maximal 50 characters");

            string uniqueField = "[Root Cause Name]";

            await using var conn = dbContext.CARConnection();

            if (conn.State == ConnectionState.Closed)
            {
                await conn.OpenAsync(); 
            }
            DataTable excelData = GlobalFunction.ReadExcelFile(filePath); 
            if (excelData.Rows.Count == 0)
            {
                return new List<string> { "No data found in the Excel file" };
            }

            // Perform structure validation
            if (!excelData.Columns.Contains("Plant") || !excelData.Columns.Contains("Root Cause Name"))
            {
                return new List<string> { "Invalid Data structure, please follow template format" };
            }

            using (var transaction = conn.BeginTransaction())
            {
                // Membuat temp table
                var createTempTable = @"CREATE TABLE ##temp (
                                Plant NVARCHAR(50),
                                [Root Cause Name] NVARCHAR(50)
                            )";
                await conn.ExecuteAsync(createTempTable, transaction: transaction);

                using (var bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkCopy.DestinationTableName = "##temp"; // Temp table
                    bulkCopy.ColumnMappings.Add("Plant", "Plant");
                    bulkCopy.ColumnMappings.Add("Root Cause Name", "Root Cause Name");
                    await bulkCopy.WriteToServerAsync(excelData);
                }

                var result = await conn.QueryAsync<string>(query, new
                {
                    FilePath = filePath,
                    ExcelCol = excelCol,
                    ExcelRange = excelRange,
                    Conditions = conditions,
                    CondRemark = condRemark,
                    SpecialCond = specialCond,
                    UniqueField = uniqueField,
                    UserId = userId
                }, transaction: transaction);

                transaction.Commit();
                return result;

            }
        }
    }   
}
