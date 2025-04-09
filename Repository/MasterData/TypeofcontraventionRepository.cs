using Contracts.Repository.MasterData;
using Dapper;
using Entities.MasterData;
using Microsoft.Data.SqlClient;
using OfficeOpenXml.Style;
using OfficeOpenXml;
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
    internal sealed class TypeofcontraventionRepository(DbContext dbContext) : ITypeOfContraventionRepository
    {
        public async Task<IEnumerable<TypeofcontraventionDto>> GetTypeofcontravention(GETTypeofcontravention GETTypeofcontravention)
        {
            string query;

            if (string.IsNullOrEmpty(GETTypeofcontravention.search) && string.IsNullOrEmpty(GETTypeofcontravention.SearchADV))
            {
                query = TypeofcontraventionQuery.GetTypeofcontravention;
            }
            else if (!string.IsNullOrEmpty(GETTypeofcontravention.SearchADV))
            {
                query = TypeofcontraventionQuery.SearchDataADV;
            }
            else
            {
                query = TypeofcontraventionQuery.SearchDataInDB;
            }

            if (!GETTypeofcontravention.delflag)
            {
                query += " and DelFlag = 0";
            }
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<TypeofcontraventionDto>(query, new { search = GETTypeofcontravention.search, SearchADV = GETTypeofcontravention.SearchADV, plant = GETTypeofcontravention.plant });
        }

        public async Task<IEnumerable<TypeofcontraventionDto>> InsertNewData(CRUDTypeofcontraventionDto CRUDTypeofcontraventionDto)
        {
            string query = TypeofcontraventionQuery.InsertNewData;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<TypeofcontraventionDto>(query, new { Typeofcontravention = CRUDTypeofcontraventionDto.Typeofcontravention, userId = CRUDTypeofcontraventionDto.userid, plant = CRUDTypeofcontraventionDto.Plant });
        }

        public async Task<IEnumerable<TypeofcontraventionDto>> UpdateData(CRUDTypeofcontraventionDto CRUDTypeofcontraventionDto)
        {
            string query = TypeofcontraventionQuery.UpdateData;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<TypeofcontraventionDto>(query, new { Typeofcontravention = CRUDTypeofcontraventionDto.Typeofcontravention, id = CRUDTypeofcontraventionDto.id, userId = CRUDTypeofcontraventionDto.userid });
        }

        public async Task<IEnumerable<TypeofcontraventionDto>> DataDelete(CRUDTypeofcontraventionDto CRUDTypeofcontraventionDto)
        {
            string query = TypeofcontraventionQuery.DeleteData;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<TypeofcontraventionDto>(query, new { id = CRUDTypeofcontraventionDto.id, userId = CRUDTypeofcontraventionDto.userid });
        }

        public async Task<IEnumerable<TypeofcontraventionDto>> DataPermDelete(CRUDTypeofcontraventionDto CRUDTypeofcontraventionDto)
        {
            string query = TypeofcontraventionQuery.PermDeleteData;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<TypeofcontraventionDto>(query, new { id = CRUDTypeofcontraventionDto.id });
        }

        public async Task<IEnumerable<TypeofcontraventionDto>> DataRecover(CRUDTypeofcontraventionDto CRUDTypeofcontraventionDto)
        {
            string query = TypeofcontraventionQuery.RecoverData;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<TypeofcontraventionDto>(query, new { id = CRUDTypeofcontraventionDto.id, userId = CRUDTypeofcontraventionDto.userid });
        }

        public async Task<byte[]> Template()
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "\\Excel";
            string fileName = string.Format("Typeofcontravention_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss"));
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
            var row2Header = new object[] { "Mandatory", "Mandatory" };
            var row3Header = new object[] { "int", "nvarchar(100)" };
            var row4Header = new object[] { "Plant", "Typeofcontravention" };
            var row5Header = new object[] { "2100", "Test" };

            var data = new List<object[]>
                {
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

        public async Task<ImportResult> Import(string filePath, string userId)
        {
            string excelCol = "Plant,Typeofcontravention ";
            string excelRange = "A4:E5000";
            string query = TypeofcontraventionQuery.Import;

            ArrayList conditions = new ArrayList();
            ArrayList condRemark = new ArrayList();
            ArrayList specialCond = new ArrayList();

            conditions.Add("ISNULL(Plant, '') = ''");
            condRemark.Add("Plant Is required");
            conditions.Add("ISNULL(Typeofcontravention, '') = ''");
            condRemark.Add("Typeofcontravention Is required");
            conditions.Add("LEN(Typeofcontravention) > 100");
            condRemark.Add("Typeofcontravention maximal 100 characters");

            string uniqueField = "Plant,Typeofcontravention";

            await using var connMDM = dbContext.MDMConnection();
            var validPlant = (await connMDM.QueryAsync<string>("select plant from tplant")).ToList();

            if (validPlant.Count > 0)
            {
                string validPlantstr = string.Join("', '", validPlant);
                conditions.Add($"UPPER(LTRIM(RTRIM(Plant))) NOT IN ('{validPlantstr}')");
                condRemark.Add($"The Plant you entered is not registered in the MDM Plant Table");
            }

            await using var conn = dbContext.CARConnection();

            if (conn.State == ConnectionState.Closed)
            {
                await conn.OpenAsync();
            }

            // Read Excel or TXT file
            ExcelReadResponseDto excelData = GlobalFunction.ReadExcelFile(filePath, userId, query, excelCol, "Typeofcontravention", "Typeofcontravention", conditions, condRemark, excelRange, uniqueField);

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

                    // Check for duplicate data
                    if (!string.IsNullOrEmpty(uniqueField))
                    {
                        sql += $@"
                        WITH cte AS (
                            SELECT {excelCol}, ROW_NUMBER() OVER (PARTITION BY {uniqueField} ORDER BY {uniqueField}) AS row_num
                            FROM ##temp
                        )
                        INSERT INTO #invaliddata ({excelCol}, [Issue Remark])
                        SELECT {excelCol}, 'Duplicate Excel Data' FROM cte WHERE row_num > 1;

                        WITH cte AS (
                            SELECT *,
                                    ROW_NUMBER() OVER (PARTITION BY {uniqueField} ORDER BY (SELECT NULL)) AS row_num
                            FROM ##temp
                        )
                        DELETE FROM cte WHERE row_num > 1";

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
