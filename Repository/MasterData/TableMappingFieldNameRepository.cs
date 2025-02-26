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
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Repository.MasterData
{
    internal sealed class TableMappingFieldNameRepository(DbContext dbContext) :ITableMappingFieldNameRepository
    {
        public async Task<IEnumerable<TableMappingFieldNameDto>> GetTableMappingFieldName(string? search, string? SearchADVFN, string? SearchADVUID, string? SearchADVLANG, bool delflag, string? plant)
        {
            string query;

            if (string.IsNullOrEmpty(search) && string.IsNullOrEmpty(SearchADVFN) && string.IsNullOrEmpty(SearchADVUID) && string.IsNullOrEmpty(SearchADVLANG))
            {
                query = TableMappingFieldNameQuery.GetTableMappingFieldName;
            }
            else if (!string.IsNullOrEmpty(SearchADVFN) || !string.IsNullOrEmpty(SearchADVUID) || !string.IsNullOrEmpty(SearchADVLANG))
            {
                query = TableMappingFieldNameQuery.SearchDataADV;
                if(!string.IsNullOrEmpty(SearchADVUID))
                {
                    query += " and UIDisplay LIKE '%' + @SearchADVUID + '%'";
                }
                else if (!string.IsNullOrEmpty(SearchADVLANG))
                {
                    query += "and Language LIKE '%' + @SearchADVLANG + '%'";
                }
            }
            else
            {
                query = TableMappingFieldNameQuery.SearchDataInDB;
            }

            if (!delflag)
            {
                query += " and DelFlag = 0";
            }
            if(!string.IsNullOrEmpty(plant))
            {
                query += " and Plant = @plant";
            }
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<TableMappingFieldNameDto>(query, new { search = search, SearchADVFN = SearchADVFN, SearchADVUID = SearchADVUID, SearchADVLANG = SearchADVLANG, Plant = plant });
        }

        public async Task<IEnumerable<TableMappingFieldNameDto>> CheckExistingData(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto)
        {
            string PlantParam = CRUDTableMappingFieldNameDto.Plant;
            string FieldNameParam = CRUDTableMappingFieldNameDto.FieldName;
            string LanguageParam = CRUDTableMappingFieldNameDto.Language;

            string query;
            query = TableMappingFieldNameQuery.CheckExistingData;

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<TableMappingFieldNameDto>(query, new { Plant = PlantParam, FieldName = FieldNameParam, Language = LanguageParam });
        }

        public async Task<IEnumerable<TableMappingFieldNameDto>> InsertNewData(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto)
        {
            string query = TableMappingFieldNameQuery.InsertNewData;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<TableMappingFieldNameDto>(query, new { FieldName = CRUDTableMappingFieldNameDto.FieldName, UIDisplay = CRUDTableMappingFieldNameDto.UIDisplay, Language = CRUDTableMappingFieldNameDto.Language, userId = CRUDTableMappingFieldNameDto.userid, plant = CRUDTableMappingFieldNameDto.Plant });
        }

        public async Task<IEnumerable<TableMappingFieldNameDto>> UpdateData(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto)
        {
            string query = TableMappingFieldNameQuery.UpdateData;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<TableMappingFieldNameDto>(query, new { UIDisplay = CRUDTableMappingFieldNameDto.UIDisplay, Language = CRUDTableMappingFieldNameDto.Language, id = CRUDTableMappingFieldNameDto.ID, userId = CRUDTableMappingFieldNameDto.userid });
        }

        public async Task<IEnumerable<TableMappingFieldNameDto>> DataDelete(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto)
        {
            string query = TableMappingFieldNameQuery.DeleteData;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<TableMappingFieldNameDto>(query, new { id = CRUDTableMappingFieldNameDto.ID, userId = CRUDTableMappingFieldNameDto.userid });
        }

        public async Task<IEnumerable<TableMappingFieldNameDto>> DataPermDelete(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto)
        {
            string query = TableMappingFieldNameQuery.PermDeleteData;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<TableMappingFieldNameDto>(query, new { id = CRUDTableMappingFieldNameDto.ID });
        }

        public async Task<IEnumerable<TableMappingFieldNameDto>> DataRecover(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto)
        {
            string query = TableMappingFieldNameQuery.RecoverData;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<TableMappingFieldNameDto>(query, new { id = CRUDTableMappingFieldNameDto.ID, userId = CRUDTableMappingFieldNameDto.userid });
        }

        public async Task<byte[]> Template()
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "\\Excel";
            string fileName = string.Format("TableMappingFieldName_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss"));
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
            var row2Header = new object[] { "Mandatory", "Mandatory", "Mandatory", "Mandatory" };
            var row3Header = new object[] { "int", "nvarchar(100)", "nvarchar(100)", "nvarchar(50)" };
            var row4Header = new object[] { "Plant", "FieldName", "UIDisplay", "Language" };
            var row5Header = new object[] {"2100", "Test", "Test", "EN" };

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
            using (var range = sheet.Cells[1, 1, 3, 4])
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
            string excelCol = "Plant, FieldName, UIDisplay, Language";
            string excelRange = "A4:E5000";
            string query = TableMappingFieldNameQuery.Import;

            ArrayList conditions = new ArrayList();
            ArrayList condRemark = new ArrayList();
            ArrayList specialCond = new ArrayList();

            List<string> mandatoryFields = new List<string> { "Plant", "FieldName", "UIDisplay", "Language" };

            await using var conn = dbContext.CARConnection();
            if (conn.State == ConnectionState.Closed)
            {
                await conn.OpenAsync();
            }

            var validLanguages = (await conn.QueryAsync<string>(
           "USE MDM;SELECT idvalue FROM TGLOBAL WHERE id = 'LanguageOptions'"

            )).ToList();

            var validFieldName = (await conn.QueryAsync<string>(
                "USE CAR;Select COLUMN_NAME AS ColumnName FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'IssueFeedback' ORDER BY ORDINAL_POSITION")).ToList();

            var validPlant = (await conn.QueryAsync<string>(
               "USE MDM;select plant from tplant")).ToList();

                        //conditions.Add("ISNULL(Plant, '') = '' OR ISNULL(FieldName, '') = '' OR ISNULL(UIDisplay, '') = '' OR ISNULL(Language, '') = ''");
                        //condRemark.Add("Null Mandatory FieldName Data");
                        //List<string> mandatoryFields = new List<string> { "Plant", "FieldName", "UIDisplay", "Language" };
                        List<string> validLanguageList = new List<string>();

            if (validLanguages.Count > 0)
            {
                validLanguageList = validLanguages[0]
                    .Split(',')
                    .Select(lang => lang.Trim().ToUpper())
                    .ToList();
            }

            if (validFieldName.Count > 0)
            {
                string validFieldNameStr = string.Join("', '", validFieldName); // Format: 'EN', 'ZH'
                conditions.Add($"UPPER(LTRIM(RTRIM(FieldName))) NOT IN ('{validFieldNameStr}')");
                condRemark.Add($"Field Name is not Existing in IssueFeedBack");
            }

            if(validPlant.Count > 0)
            {
                string validPlantstr = string.Join("', '", validPlant);
                conditions.Add($"UPPER(LTRIM(RTRIM(Plant))) NOT IN ('{validPlantstr}')");
                condRemark.Add($"Plant is not Existing IN TPLANT");
            }
        

            if (validLanguageList.Count > 0)
            {
                string validLanguageStr = string.Join("', '", validLanguageList);

                // Perbaiki kondisi SQL
                conditions.Add($"UPPER(LTRIM(RTRIM(Language))) NOT IN ('{validLanguageStr}')");
                condRemark.Add($"Language value only {string.Join(" and ", validLanguageList)}");
            }

            foreach (var field in mandatoryFields)
            {
                conditions.Add($"ISNULL({field}, '') = ''");
                condRemark.Add($"{field} is required");
            }
            conditions.Add("Plant <> '' AND TRY_CAST(Plant AS INT) IS NULL");
            condRemark.Add("Plant Value Must Be a number");
            conditions.Add("LEN(FieldName) > 100");
            condRemark.Add("FieldName maximal 100 characters");
            conditions.Add("LEN(UIDisplay) > 100");
            condRemark.Add("UIDisplay maximal 100 characters");
            conditions.Add("LEN(Language) > 50");
            condRemark.Add("Language maximal 100 characters");

            string uniqueField = "FieldName,Plant,Language";

            //await using var conn = dbContext.CARConnection();

            if (conn.State == ConnectionState.Closed)
            {
                await conn.OpenAsync();
            }

            // Read Excel or TXT file
            ExcelReadResponseDto excelData = GlobalFunction.ReadExcelFile(filePath, userId, query, excelCol, "TableMappingFieldName", "TableMappingFieldName", conditions, condRemark, excelRange, uniqueField);

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
                        SELECT {excelCol}, 'Duplicate Data' FROM cte WHERE row_num > 1;

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
