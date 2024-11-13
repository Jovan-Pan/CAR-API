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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Repository.MasterData
{
    internal sealed class NCTextSentenceRepository(DbContext dbContext) : INCTextSentenceRepository
    {
        public async Task<IEnumerable<NCTextSentenceDto>> GetDataNcTextSentence(string search, string SearchADV, bool delflag)
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

            if (!delflag)
            {
                query += " and DelFlag = 0";
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

        public async Task<ImportResult> Import(string filePath, string userId)
        {
            string excelCol = "TextSentence, isFirstSentence, isLastSentence";
            string excelRange = "A4:E5000";
            string query = NCTextSentenceQuery.Import;
            ArrayList conditions = new ArrayList();
            ArrayList condRemark = new ArrayList();
            ArrayList specialCond = new ArrayList();
            specialCond.Add("UPDATE ##temp set isFirstSentence = (CASE isFirstSentence WHEN 'Y' then 'true' when 'N' then 'false' else isFirstSentence end); ");
            specialCond.Add("UPDATE ##temp set isLastSentence = (CASE isLastSentence WHEN 'Y' then 'true' when 'N' then 'false' else isLastSentence end); ");

            conditions.Add(" ISNULL(TextSentence, '') = ''  or ISNULL(isFirstSentence, '') = '' or ISNULL(isLastSentence, '') = ''  ");
            condRemark.Add("Null Mandatory Data");
            conditions.Add(" LEN(TextSentence) > 100");
            condRemark.Add("TextSentence maximal 100 characters");
            conditions.Add("isFirstSentence not in ('Y','N') ");
            condRemark.Add("isFirstSentence value is Y or N");
            conditions.Add("isLastSentence not in ('Y','N') ");
            condRemark.Add("isLastSentence value is Y or N");

            string uniqueField = "TextSentence";

            await using var conn = dbContext.CARConnection();

            if (conn.State == ConnectionState.Closed)
            {
                await conn.OpenAsync();
            }

            // Read Excel or TXT file
            ExcelReadResponseDto excelData = GlobalFunction.ReadExcelFile(filePath, userId, query, excelCol, "Nctextsentence", "Nctextsentence", conditions, condRemark, excelRange, uniqueField);

            if (!excelData.Success)
            {
                System.IO.File.Delete(filePath);
                return new ImportResult { Success = false, Message = "Import Failed: " + excelData.Message };
            }

            if (!excelData.DataTable.Columns.Contains("Issue Remark"))
            {
                excelData.DataTable.Columns.Add("Issue Remark", typeof(string));
            }

            System.IO.File.Delete(filePath);

            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    // Create temp table
                    var createTempTable = @"IF OBJECT_ID('tempdb..##temp') IS NOT NULL DROP TABLE ##temp; CREATE TABLE ##temp (";
                    var columnMappings = new List<string>(); // For SqlBulkCopy mappings

                    // Loop through the columns in the DataTable to create table definition and mappings
                    foreach (DataColumn column in excelData.DataTable.Columns)
                    {
                        if (!string.IsNullOrWhiteSpace(column.ColumnName))
                        {
                            createTempTable += $"[{column.ColumnName}] NVARCHAR(MAX) COLLATE DATABASE_DEFAULT, ";
                            columnMappings.Add(column.ColumnName);
                        }
                    }

                    // Append [Issue Remark] if it's not already included
                    if (!columnMappings.Contains("Issue Remark"))
                    {
                        createTempTable += "[Issue Remark] NVARCHAR(MAX) COLLATE DATABASE_DEFAULT, ";
                        columnMappings.Add("Issue Remark");
                    }

                    // Remove the last comma and space, and close the SQL statement
                    createTempTable = createTempTable.TrimEnd(',', ' ') + ")";

                    // Execute the CREATE TABLE statement
                    await conn.ExecuteAsync(createTempTable, transaction: transaction);

                    // Bulk copy data to temp table
                    using (var bulkCopy = new SqlBulkCopy((SqlConnection)conn, SqlBulkCopyOptions.Default, (SqlTransaction)transaction))
                    {
                        bulkCopy.DestinationTableName = "##temp";
                        foreach (var columnName in columnMappings)
                        {
                            bulkCopy.ColumnMappings.Add(columnName, columnName);
                        }
                        await bulkCopy.WriteToServerAsync(excelData.DataTable);
                    }

                    // Trim columns and initialize Issue Remark
                    var columnNameTrim = string.Join(", ", excelData.DataTable.Columns.Cast<DataColumn>()
                        .Select(c => $"[{c.ColumnName}] = LTRIM(RTRIM([{c.ColumnName}]))"));

                    string sql = $@"UPDATE ##temp SET {columnNameTrim}; UPDATE ##temp SET [Issue Remark] = '';";

                    // Check for invalid data
                    if (conditions != null && conditions.Count > 0)
                    {
                        sql += @" IF OBJECT_ID('tempdb..#invaliddata') IS NOT NULL DROP TABLE #invaliddata;
                                  SELECT TOP 0 * INTO #invaliddata FROM ##temp;";

                        for (int i = 0; i < conditions.Count; i++)
                        {
                            sql += $@" 
                            INSERT INTO #invaliddata ({excelCol}, [Issue Remark])
                            SELECT {excelCol}, '{condRemark[i]}'
                            FROM ##temp
                            WHERE {conditions[i]};

                            DELETE FROM ##temp WHERE {conditions[i]};";
                        }
                    }

                    var TotalCountIsFirstSentence = await conn.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM (SELECT isFirstSentence FROM ##temp WHERE isFirstSentence = 'Y' UNION ALL SELECT isFirstSentence FROM nctextsentence WHERE isFirstSentence = '1' and delflag ='0') AS CombinedData;", transaction: transaction);
                    var TotalCountIsLastSentence = await conn.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM (SELECT isLastSentence FROM ##temp WHERE isLastSentence = 'Y' UNION ALL SELECT isLastSentence FROM nctextsentence WHERE isLastSentence = '1' and delflag ='0') AS CombinedData;", transaction: transaction);
                    var CountTempIsfirstsentence = await conn.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM ##temp WHERE isFirstSentence = 'Y'", transaction: transaction);
                    var CountTempisLastSentence = await conn.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM ##temp WHERE isLastSentence = 'Y'", transaction: transaction);
                    var TempTextsenteceIsfirstsentence = await conn.ExecuteScalarAsync($"SELECT textsentence FROM ##temp WHERE isFirstSentence = 'Y'", transaction: transaction);
                    var TempTextsenteceisLastSentence = await conn.ExecuteScalarAsync($"SELECT textsentence FROM ##temp WHERE isLastSentence = 'Y'", transaction: transaction);
                    var checkTextsentenceIsfirstsentence = await conn.ExecuteScalarAsync($@"
                                                    SELECT t.TextSentence, t.isfirstsentence 
                                                FROM ##temp t 
                                                INNER JOIN NCTextSentence n 
                                                    ON t.TextSentence = n.TextSentence 
                                                WHERE 
                                                    (CASE t.isfirstsentence 
                                                        WHEN 'Y' THEN 1 
                                                     END) = CAST(n.isfirstsentence AS INT);
                                                ", transaction: transaction);
                    var checkTextsentenceisLastSentence = await conn.ExecuteScalarAsync($@"
                                                    SELECT t.TextSentence, t.isLastSentence 
                                                FROM ##temp t 
                                                INNER JOIN NCTextSentence n 
                                                    ON t.TextSentence = n.TextSentence 
                                                WHERE 
                                                    (CASE t.isLastSentence 
                                                        WHEN 'Y' THEN 1 
                                                     END) = CAST(n.isLastSentence AS INT);
                                                ", transaction: transaction);

                    if (TotalCountIsFirstSentence > 1)
                    {
                        if (CountTempIsfirstsentence > 0)
                        {
                            if (TempTextsenteceIsfirstsentence?.ToString().ToLower() != checkTextsentenceIsfirstsentence?.ToString().ToLower())
                            {
                                sql += @"
                                INSERT INTO #invaliddata (TextSentence, isFirstSentence, isLastSentence, [Issue Remark])
                                SELECT TextSentence, isFirstSentence, isLastSentence, 
                                    CASE 
                                        WHEN isFirstSentence = 'Y' AND (SELECT COUNT(*) FROM ##temp WHERE isFirstSentence = 'Y') > 0 THEN 'Only one item can be marked as the first sentence.'
                                        ELSE NULL
                                    END
                                FROM ##temp
                                WHERE (isFirstSentence = 'Y' AND (SELECT COUNT(*) FROM ##temp WHERE isFirstSentence = 'Y') > 0);

                                DELETE FROM ##temp
                                WHERE (isFirstSentence = 'Y' AND (SELECT COUNT(*) FROM ##temp WHERE isFirstSentence = 'Y') > 0);
                                ";
                            }
                        }
                    }

                    if (TotalCountIsLastSentence > 1)
                    {
                        if (CountTempisLastSentence > 0)
                        {
                            if (TempTextsenteceisLastSentence?.ToString().ToLower() != checkTextsentenceisLastSentence?.ToString().ToLower())
                            {
                                sql += @"
                                INSERT INTO #invaliddata (TextSentence, isFirstSentence, isLastSentence, [Issue Remark])
                                SELECT TextSentence, isFirstSentence, isLastSentence, 
                                    CASE 
                                        WHEN isLastSentence = 'Y' AND (SELECT COUNT(*) FROM ##temp WHERE isLastSentence = 'Y') > 0 THEN 'Only one item can be marked as the last sentence.'
                                        ELSE NULL
                                    END
                                FROM ##temp
                                WHERE (isLastSentence = 'Y' AND (SELECT COUNT(*) FROM ##temp WHERE isLastSentence = 'Y') > 0);

                                DELETE FROM ##temp
                                WHERE (isLastSentence = 'Y' AND (SELECT COUNT(*) FROM ##temp WHERE isLastSentence = 'Y') > 0);
                                ";
                            }
                        }
                    }

                    // Check for duplicate data
                    if (!string.IsNullOrEmpty(uniqueField))
                    {
                        sql += $@"
                        WITH cte AS (
                            SELECT {excelCol}, ROW_NUMBER() OVER (PARTITION BY {uniqueField} ORDER BY {uniqueField}) AS row_num
                            FROM ##temp
                        )
                        INSERT INTO #invaliddata ({excelCol}, [Issue Remark])
                        SELECT {excelCol}, 'Duplicate Data' FROM cte WHERE row_num > 1;

                        DELETE FROM ##temp WHERE {uniqueField} IN (
                            SELECT {uniqueField} FROM(
                                                SELECT {uniqueField}
                                                FROM ##temp
                                                GROUP BY {uniqueField}
                                                HAVING COUNT(*) > 1
                                            ) AS duplicates
                                        )";

                    }

                    // Execute special conditions if any
                    if (specialCond != null && specialCond.Count > 0)
                    {
                        foreach (string specialCondition in specialCond)
                        {
                            sql += specialCondition;
                        }
                    }

                    // Execute the SQL command
                    await conn.ExecuteAsync(sql, transaction: transaction);

                    // Fetch invalid data
                    var invalidData = await conn.QueryAsync($"SELECT * FROM #invaliddata", transaction: transaction);

                    List<Dictionary<string, object>> dataListInValid = invalidData
                     .Select(row => new Dictionary<string, object>(row))
                     .ToList();

                    // Count valid data
                    var validDataCount = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM ##temp", transaction: transaction);

                    if (validDataCount == 0 && dataListInValid.Count == 0)
                    {
                        transaction.Rollback();
                        return new ImportResult { Success = false, Message = "No Data Found" };
                    }
                    else
                    {
                        string validmsg = "Import Success";
                        string validlabel = "Success";
                        if (dataListInValid.Count != 0)
                        {
                            validmsg = "Upload success with error. Refer to Import Summary";
                            validlabel = "Partial Success";
                        }

                        // Proceed to insert valid data
                        await conn.QueryAsync<string>(query, new
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
                        return new ImportResult
                        {
                            Success = true,
                            DataInvalid = dataListInValid,
                            DataValidCount = validDataCount,
                            DataInvalidCount = dataListInValid.Count,
                            Message = validmsg,
                            Label = validlabel
                        };
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    // Log the error if necessary
                    return new ImportResult { Success = false, Message = "Error occurred: " + ex.Message };
                }
            }
        }
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         