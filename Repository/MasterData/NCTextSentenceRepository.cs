using Contracts.Repository.MasterData;
using Dapper;
using Entities.MasterData;
using Microsoft.Data.SqlClient;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Repository.Query;
using Services.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.MasterData
{
    internal sealed class NCTextSentenceRepository(DbContext dbContext) : INCTextSentenceRepository
    {
        public async Task<IEnumerable<NCTextSentenceDto>> GetDataNcTextSentence(string search, string SearchADV)
        {
            string query;

            if (string.IsNullOrEmpty(search) && string.IsNullOrEmpty(SearchADV))
            {
                query = NCTextSentenceQuery.GetDataNcTextSentence;
            }
            else if (!string.IsNullOrEmpty(SearchADV))
            {
                query = NCTextSentenceQuery.SearchadvData;
            }
            else
            {
                query = NCTextSentenceQuery.SearchDatainDB;
            }
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<NCTextSentenceDto>(query, new { search = search, SearchADV = SearchADV });
        }
        public async Task<IEnumerable<NCTextSentenceDto>> InsertDataNcTextSentence(string TextSentence, bool isFirstSentence, bool isLastSentence, string userId)
        {
            string query = NCTextSentenceQuery.InsertDataNcTextSentence;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<NCTextSentenceDto>(query, new { TextSentence = TextSentence, isFirstSentence = isFirstSentence, isLastSentence = isLastSentence, userId = userId });
        }
        public async Task<IEnumerable<NCTextSentenceDto>> UpdateDataNcTextSentence(int id, string TextSentence, bool isFirstSentence, bool isLastSentence, string userId)
        {
            string query = NCTextSentenceQuery.UpdateDataNcTextSentence;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<NCTextSentenceDto>(query, new { id = id, TextSentence = TextSentence, isFirstSentence = isFirstSentence, isLastSentence = isLastSentence, userId = userId });
        }
        public async Task<IEnumerable<NCTextSentenceDto>> DataDelete(int id, string userId)
        {
            string query = NCTextSentenceQuery.DataDelete;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<NCTextSentenceDto>(query, new { id = id, userId = userId });
        }
        public async Task<IEnumerable<NCTextSentenceDto>> DataPermDelete(int id)
        {
            string query = NCTextSentenceQuery.DataPermDelete;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<NCTextSentenceDto>(query, new { id = id });    
        }
        public async Task<IEnumerable<NCTextSentenceDto>> DataRecover(int id, string userId)
        {
            string query = NCTextSentenceQuery.DataRecover;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<NCTextSentenceDto>(query, new { id = id, userId = userId });
        }
        public async Task<byte[]> Template()
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "\\Excel";
            string fileName = string.Format("NcTextSentence_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string fullPath = string.Format("{0}\\{1}", filePath, fileName);

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            using (var excel = new ExcelPackage(new FileInfo(fullPath)))
            {
                var sheet1 = excel.Workbook.Worksheets.Add("Template");

                WriteTemplateContent(sheet1);

                await excel.SaveAsync();
            }

            MemoryStream ms = new();
            using (FileStream file = new(fullPath, FileMode.Open, FileAccess.Read))
            {
                await file.CopyToAsync(ms);
                ms.Position = 0;
            }
            return await File.ReadAllBytesAsync(fullPath);
        }

        private static void WriteTemplateContent(ExcelWorksheet sheet)
        {
            var row1Header = new object[] { "(Please Don't Delete Highlighted Row)" };
            var row2Header = new object[] { "Mandatory", "Mandatory", "Mandatory" };
            var row3Header = new object[] { "nvarchar(100)", "Bit(Y/N)", "Bit(Y/N)"};
            var row4Header = new object[] { "TextSentence", "isFirstSentence", "isLastSentence" };
            var row5Header = new object[] { "Test", "Y", "N" };

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
            using (var range = sheet.Cells[1, 1, 3, 3])
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
            const int startFromRow = 1; // Skip the first two rows asheaders
            int endRow = data.Count; // Last row in the worksheet
            const int columnNumber = 1; // Column D
            var columnRange = sheet.Cells[startFromRow, columnNumber, endRow, columnNumber];

            columnRange.Style.Numberformat.Format = "mm/dd/yyyy";
        }

        public async Task<IEnumerable<string>> Import(string filePath, string userId)
        {
            string excelCol = "TextSentence, isFirstSentence, isLastSentence";
            string excelRange = "A4:E5000";
            string query = NCTextSentenceQuery.Import;
            ArrayList conditions = new ArrayList();
            ArrayList condRemark = new ArrayList();
            ArrayList specialCond = new ArrayList();
            conditions.Add(" ISNULL(TextSentence, '') = ''  or ISNULL(isFirstSentence, '') = '' or ISNULL(isLastSentence, '') = ''  ");
            condRemark.Add("Null Mandatory Data");
            conditions.Add(" LEN(TextSentence) > 100");
            condRemark.Add("TextSentence maximal 100 characters");

            string uniqueField = "TextSentence";

            await using var conn = dbContext.CARConnection();

            if (conn.State == ConnectionState.Closed)
            {
                await conn.OpenAsync();
            }
            DataTable excelData = GlobalFunction.ReadExcelFile(filePath);

            using (var transaction = conn.BeginTransaction())
            {
                // Membuat temp table
                var createTempTable = @"CREATE TABLE ##temp (
                                TextSentence nvarchar(100),
                                isFirstSentence bit,
                                isLastSentence bit
                            )";
                await conn.ExecuteAsync(createTempTable, transaction: transaction);

                foreach (DataRow row in excelData.Rows)
                {
                    if (excelData.Columns.Contains("isFirstSentence"))
                    {
                        // Convert '0' or '1' strings to boolean values (bit in SQL)
                        row["isFirstSentence"] = row["isFirstSentence"].ToString() == "Y" ? true : false;
                    }
                    if (excelData.Columns.Contains("isLastSentence"))
                    {
                        // Convert '0' or '1' strings to boolean values (bit in SQL)
                        row["isLastSentence"] = row["isLastSentence"].ToString() == "Y" ? true : false;
                    }
                }
                using (var bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkCopy.DestinationTableName = "##temp"; // Temp table
                    bulkCopy.ColumnMappings.Add("TextSentence", "TextSentence");
                    bulkCopy.ColumnMappings.Add("isFirstSentence", "isFirstSentence");
                    bulkCopy.ColumnMappings.Add("isLastSentence", "isLastSentence");
                    await bulkCopy.WriteToServerAsync(excelData);
                    //await bulkCopy.WriteToServerAsync(excelData);
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
