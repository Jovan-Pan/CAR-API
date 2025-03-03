using Contracts.Repository.CAR;
using Dapper;
using Entities.CAR;
using Entities.MasterData;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.FileIO;
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
using Entities.ParamRequest;
using System.Text.RegularExpressions;

namespace Repository.CAR
{
    internal sealed class DynamicFormConfigurationRepository(DbContext dbContext) :IDynamicFormConfigurationRepository
    {
        public async Task<IEnumerable<IssueFeedbackColumnInfo>> GetIssueFeedbackColumn()
        {
            string query = DynamicFormConfigurationQuery.GetIssueFeedbackcolumn;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<IssueFeedbackColumnInfo>(query);

        }
        public async Task<IEnumerable<DynamicFormConfigurationDto>> GetTestingQueryResult(TestingQueryParam TestingQueryParam)
        {
            await using var conn = dbContext.CARConnection();
            string queryLowerCase = TestingQueryParam.query.ToLower();
            string queryWithPlantReplaced;

            if (queryLowerCase.Contains("WHERE", StringComparison.OrdinalIgnoreCase))
            {
                string takeParameterDB = "SELECT AllowParameters FROM AllowParameters";
                var executedParameters = await conn.QueryAsync<string>(takeParameterDB);

                HashSet<string> allowedParams = executedParameters
                                      .Select(p => p.ToLower()) 
                                      .ToHashSet();

                var regex = new Regex(@"@\w+", RegexOptions.IgnoreCase);
                var foundParams = regex.Matches(TestingQueryParam.query)
                                       .Select(match => match.Value.ToLower())
                                       .ToHashSet();

                var invalidParams = foundParams.Except(allowedParams);
                if (invalidParams.Any())
                {
                    return new List<DynamicFormConfigurationDto>
                    {
                        
                    };
                }

                queryWithPlantReplaced = TestingQueryParam.query.ToLower()
                                            .Replace("@plant", TestingQueryParam.plant)
                                            .Replace("@userid", $"'{TestingQueryParam.userid}'");
            }
            else
            {
                queryWithPlantReplaced = TestingQueryParam.query;
            }
            string executedQuery = "USE " + TestingQueryParam.DBResource + "; " + queryWithPlantReplaced;
            var executedQueryResult = await conn.QueryAsync<dynamic>(executedQuery);

            // Initialize the list if it is null
            var result = new List<DynamicFormConfigurationDto>();

            foreach (var row in executedQueryResult)
            {
                var item = new DynamicFormConfigurationDto
                {
                    ExecutedQueryResult = new List<Dictionary<string, object>>()
                };

                var executedQueryResultDict = new Dictionary<string, object>();

                foreach (var kvp in (IDictionary<string, object>)row)
                {
                    if (kvp.Key != null)
                    {
                        executedQueryResultDict[kvp.Key] = kvp.Value;
                    }
                }

                item.ExecutedQueryResult.Add(executedQueryResultDict);
                result.Add(item);
            }
            return result;
        }
        public async Task<IEnumerable<DynamicFormConfigurationDto>> InsertNewData(DynamicFormConfigurationDto DynamicFormConfigurationDto)
            {
            string query = DynamicFormConfigurationQuery.InsertNewData;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<DynamicFormConfigurationDto>(query, new 
            {
                plant = DynamicFormConfigurationDto.plant,
                FormType = DynamicFormConfigurationDto.FormType,
                FieldName = DynamicFormConfigurationDto.FieldName,
                FieldType = DynamicFormConfigurationDto.FieldType,
                FieldLength = DynamicFormConfigurationDto.FieldLength,
                Mandatory = DynamicFormConfigurationDto.Mandatory,
                FieldElement = DynamicFormConfigurationDto.FieldElement,
                OptionDataResource = DynamicFormConfigurationDto.OptionDataResource,
                DBResource = DynamicFormConfigurationDto.DBResource,
                Query = DynamicFormConfigurationDto.Query,
                DataOption = DynamicFormConfigurationDto.DataOption,
                Sequence = DynamicFormConfigurationDto.Sequence,
                userid = DynamicFormConfigurationDto.userid

            });
        }
        public async Task<IEnumerable<DynamicFormConfigurationDto>> GetDynamicFormConfiguration(string userid, string Language, string Plant,string? search, bool delflag, string? formTypeAdv, string? fieldNameAdv, string? fieldTypeAdv, string? fieldLengthAdv, string? mandatoryAdv, string? fieldElementAdv, string? optionDataResourceAdv, string? dbResourceAdv, string? queryAdv, string? dataOptionAdv, string? sequenceAdv)
        {
            string query;

            if (string.IsNullOrEmpty(search) && string.IsNullOrEmpty(formTypeAdv) && string.IsNullOrEmpty(fieldNameAdv) && string.IsNullOrEmpty(fieldTypeAdv) &&string.IsNullOrEmpty(fieldLengthAdv) && string.IsNullOrEmpty(mandatoryAdv) &&string.IsNullOrEmpty(fieldElementAdv) && string.IsNullOrEmpty(optionDataResourceAdv) &&string.IsNullOrEmpty(dbResourceAdv) && string.IsNullOrEmpty(queryAdv) &&string.IsNullOrEmpty(dataOptionAdv) && string.IsNullOrEmpty(sequenceAdv))
            {
                query = DynamicFormConfigurationQuery.GetDynamicFormConfiguration;
                if (!delflag)
                {
                    query += " and dfc.DelFlag = 0 ";
                }
            }
            else if (!string.IsNullOrEmpty(formTypeAdv) || !string.IsNullOrEmpty(fieldNameAdv) || !string.IsNullOrEmpty(fieldTypeAdv) || !string.IsNullOrEmpty(fieldLengthAdv) || !string.IsNullOrEmpty(mandatoryAdv) || !string.IsNullOrEmpty(fieldElementAdv) || !string.IsNullOrEmpty(optionDataResourceAdv) || !string.IsNullOrEmpty(dbResourceAdv) || !string.IsNullOrEmpty(queryAdv) || !string.IsNullOrEmpty(dataOptionAdv) || !string.IsNullOrEmpty(sequenceAdv))
            {
                query = DynamicFormConfigurationQuery.SearchADV;
                if (!delflag)
                {
                    query += " and DelFlag = 0 ";
                }
            }
            else
            {
                query = DynamicFormConfigurationQuery.SearchDB;
                if (!delflag)
                {
                    query += " and DelFlag = 0 ";
                }
            }

            await using var conn = dbContext.CARConnection();
            //return await conn.QueryAsync<DynamicFormConfigurationDto>(query, new {
            var result = await conn.QueryAsync<DynamicFormConfigurationDto>(query, new 
            {
            Language = Language,
            Plant = Plant ,
            userid = userid,
            search = search,
            formTypeAdv = formTypeAdv,
            fieldNameAdv = fieldNameAdv,
            fieldTypeAdv = fieldTypeAdv,
            fieldLengthAdv = fieldLengthAdv,
            mandatoryAdv = !string.IsNullOrEmpty(mandatoryAdv) && (mandatoryAdv.ToLower() == "true" || mandatoryAdv.ToLower() == "false")? (mandatoryAdv.ToLower() == "true" ? 0 : 1): (int?)null,
            fieldElementAdv = fieldElementAdv,
            optionDataResourceAdv = optionDataResourceAdv,
            dbResourceAdv = dbResourceAdv,
            queryAdv = queryAdv,
            dataOptionAdv = dataOptionAdv,
            sequenceAdv = sequenceAdv
            });


            //foreach (var item in result)
            //{
            //    if (item.OptionDataResource == "Execute Query")
            //    {
            //        string queryLowerCase = item.Query.ToLower();
            //        string queryWithPlantReplaced;

            //        if (queryLowerCase.Contains("WHERE", StringComparison.OrdinalIgnoreCase))
            //        {
            //            string takeParameterDB = "SELECT AllowParameters FROM AllowParameters";
            //            var executedParameters = await conn.QueryAsync<string>(takeParameterDB);

            //            HashSet<string> allowedParams = executedParameters
            //                                  .Select(p => p.ToLower())
            //                                  .ToHashSet();

            //            var regex = new Regex(@"@\w+", RegexOptions.IgnoreCase);
            //            var foundParams = regex.Matches(item.Query)
            //                                   .Select(match => match.Value.ToLower())
            //                                   .ToHashSet();

            //            var invalidParams = foundParams.Except(allowedParams);
            //            if (invalidParams.Any())
            //            {
            //                return new List<DynamicFormConfigurationDto>
            //                {

            //                };
            //            }

            //            queryWithPlantReplaced = item.Query.ToLower()
            //                                        .Replace("@plant", Plant)
            //                                        .Replace("@userid", $"'{userid}'");
                        
            //        }
            //        else
            //        {
            //            queryWithPlantReplaced = item.Query;
            //        }
            //        string executedQuery = "USE " + item.DBResource + "; " + queryWithPlantReplaced;
            //        var executedQueryResult = await conn.QueryAsync<dynamic>(executedQuery);

            //        // Initialize the list if it is null
            //        if (item.ExecutedQueryResult == null)
            //        {
            //            item.ExecutedQueryResult = new List<Dictionary<string, object>>();
            //        }

            //        // Accumulate rows from the executed query result
            //        foreach (var row in executedQueryResult)
            //        {
            //            var executedQueryResultDict = new Dictionary<string, object>();

            //            foreach (var kvp in (IDictionary<string, object>)row)
            //            {
            //                if (kvp.Key != null)
            //                {
            //                    executedQueryResultDict[kvp.Key] = kvp.Value;
            //                }
            //            }

            //            item.ExecutedQueryResult.Add(executedQueryResultDict);
            //        }
            //    }
            //}

            return result;
        }

        public async Task<IEnumerable<DynamicFormConfigurationDto>> UpdateData(DynamicFormConfigurationDto DynamicFormConfigurationDto)
        {
            string query = DynamicFormConfigurationQuery.UpdateData;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<DynamicFormConfigurationDto>(query, new { OptionDataResource = DynamicFormConfigurationDto.OptionDataResource, id = DynamicFormConfigurationDto.Id, userId = DynamicFormConfigurationDto.userid, sequence = DynamicFormConfigurationDto.Sequence, FormType = DynamicFormConfigurationDto.FormType, FieldElement = DynamicFormConfigurationDto.FieldElement ,DBResource = DynamicFormConfigurationDto.DBResource, Query = DynamicFormConfigurationDto.Query, DataOption = DynamicFormConfigurationDto.DataOption, plant = DynamicFormConfigurationDto.plant});
        }

        public async Task<IEnumerable<DynamicFormConfigurationDto>> DataDelete(DynamicFormConfigurationDto DynamicFormConfigurationDto)
        {
            string query = DynamicFormConfigurationQuery.DeleteData;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<DynamicFormConfigurationDto>(query, new { id = DynamicFormConfigurationDto.Id, userId = DynamicFormConfigurationDto.userid });
        }

        public async Task<IEnumerable<DynamicFormConfigurationDto>> DataPermDelete(DynamicFormConfigurationDto DynamicFormConfigurationDto)
        {
            string query = DynamicFormConfigurationQuery.PermDeleteData;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<DynamicFormConfigurationDto>(query, new { id = DynamicFormConfigurationDto.Id });
        }

        public async Task<IEnumerable<DynamicFormConfigurationDto>> DataRecover(DynamicFormConfigurationDto DynamicFormConfigurationDto)
        {
            string query = DynamicFormConfigurationQuery.RecoverData;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<DynamicFormConfigurationDto>(query, new { id = DynamicFormConfigurationDto.Id, userId = DynamicFormConfigurationDto.userid, sequence = DynamicFormConfigurationDto.Sequence });
        }

        public async Task<byte[]> Template()
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "\\Excel";
            string fileName = string.Format("DynamicFormConfiguration_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss"));
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
            var row2Header = new object[] { "Mandatory", "Mandatory", "Mandatory", "Mandatory", "Mandatory", "Mandatory", "Mandatory", "Mandatory", "Mandatory", "Mandatory", "Mandatory", "Mandatory" };
            var row3Header = new object[] { "int", "NVARCHAR(50)", "NVARCHAR(50)", "NVARCHAR(50)", "INT", "Bit(Y/N)", "NVARCHAR(50)", "NVARCHAR(50)", "NVARCHAR(50)", "NVARCHAR(max)", "NVARCHAR(max)", "NVARCHAR(100)" };
            var row4Header = new object[] { "Plant","FormType","FieldName","FieldType","FieldLength","Mandatory","FieldElement","OptionDataResource","DBResource","Query","DataOption","Sequence" };
            var row5Header = new object[] { "2100", "Test", "Test","string","100",1,"Dropdown","Input Manual","","","test1,test2,test3",1};

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
            using (var range = sheet.Cells[1, 1, 3, 12])
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
            string excelCol = "Plant,FormType,FieldName,FieldType,FieldLength,Mandatory,FieldElement,OptionDataResource,DBResource,Query,DataOption,Sequence";
            string excelRange = "A4:E5000";
            string query = DynamicFormConfigurationQuery.Import;

            ArrayList conditions = new ArrayList();
            ArrayList condRemark = new ArrayList();
            ArrayList specialCond = new ArrayList();

            await using var conn = dbContext.CARConnection();

            if (conn.State == ConnectionState.Closed)
            {
                await conn.OpenAsync();
            }

            specialCond.Add("UPDATE ##temp set Mandatory = (CASE Mandatory WHEN 'Y' then 'true' when 'N' then 'false' else Mandatory end); ");

            conditions.Add("ISNULL(Plant, '') = ''");
            condRemark.Add("Plant is mandatory");

            conditions.Add("ISNULL(FormType, '') = ''");
            condRemark.Add("FormType is mandatory");

            conditions.Add("ISNULL(FieldName, '') = ''");
            condRemark.Add("FieldName is mandatory");

            conditions.Add("ISNULL(FieldType, '') = ''");
            condRemark.Add("FieldType is mandatory");

            conditions.Add("FieldLength IS NOT NULL AND FieldLength < 0");
            condRemark.Add("FieldLength cannot be negative");

            conditions.Add("ISNULL(FieldElement, '') = ''");
            condRemark.Add("FieldElement is mandatory");

            conditions.Add("ISNULL(OptionDataResource, '') = ''");
            condRemark.Add("OptionDataResource is mandatory");

            conditions.Add(" Mandatory not in ('Y','N') ");
            condRemark.Add("Mandatory value is Y or N");

            conditions.Add("LEN(FormType) > 50");
            condRemark.Add("FormType maximum length is 50 characters");

            conditions.Add("LEN(FieldName) > 50");
            condRemark.Add("FieldName maximum length is 50 characters");

            conditions.Add("LEN(FieldType) > 50");
            condRemark.Add("FieldType maximum length is 50 characters");

            conditions.Add("LEN(FieldElement) > 50");
            condRemark.Add("FieldElement maximum length is 50 characters");

            conditions.Add("LEN(OptionDataResource) > 50");
            condRemark.Add("OptionDataResource maximum length is 50 characters");

            conditions.Add("LEN(DBResource) > 50");
            condRemark.Add("DBResource maximum length is 50 characters");

            conditions.Add("LEN(Sequence) > 100");
            condRemark.Add("Sequence maximum length is 100 characters");

            //conditions.Add("(SELECT COUNT(*) FROM DynamicFormConfiguration WHERE Sequence = @Sequence) <= 1");
            //condRemark.Add("Sequence must be unique");

            //conditions.Add("(SELECT COUNT(*) FROM DynamicFormConfiguration WHERE FormType = @FormType AND FieldName = @FieldName) = 0");
            //condRemark.Add("FormType and FieldName combination must be unique");

            string uniqueField = "Plant,FormType,FieldName";

            var validFieldName = (await conn.QueryAsync<string>(
                "USE CAR;Select COLUMN_NAME AS ColumnName FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'IssueFeedback' ORDER BY ORDINAL_POSITION")).ToList();

            var validFormType = (await conn.QueryAsync<string>(
                "use MDM;select IDValue from tGlobal where ID = 'CARDynamicFormTypeOption'")).ToList();

            if (validFieldName.Count > 0)
            {
                string validFieldNameStr = string.Join("', '", validFieldName); // Format: 'EN', 'ZH'
                conditions.Add($"UPPER(LTRIM(RTRIM(FieldName))) NOT IN ('{validFieldNameStr}')");
                condRemark.Add($"Field Name is not Existing in IssueFeedBack");
            }

            List<string> validFormTypeList = new List<string>();

            if (validFormTypeList.Count > 0)
            {
                validFormTypeList = validFormTypeList[0]
                    .Split(',')
                    .Select(lang => lang.Trim().ToUpper())
                    .ToList();
            }

            if (validFormTypeList.Count > 0)
            {
                string validFormTypeStr = string.Join("', '", validFormTypeList);

                conditions.Add($"UPPER(LTRIM(RTRIM(Language))) NOT IN ('{validFormTypeStr}')");
                condRemark.Add($"FormType is not Existing in Setting");
            }

            var validPlant = (await conn.QueryAsync<string>(
               "USE MDM;select plant from tplant")).ToList();

            if (validPlant.Count > 0)
            {
                string validPlantstr = string.Join("', '", validPlant);
                conditions.Add($"UPPER(LTRIM(RTRIM(Plant))) NOT IN ('{validPlantstr}')");
                condRemark.Add($"Plant is not Existing IN TPLANT");
            }


            // Read Excel or TXT file
            ExcelReadResponseDto excelData = GlobalFunction.ReadExcelFile(filePath, userId, query, excelCol, "DynamicFormConfiguration", "DynamicFormConfiguration", conditions, condRemark, excelRange, uniqueField);

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
                    if (specialCond != null && specialCond.Count > 0)
                    // Execute special conditions if any
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
        public async Task<IEnumerable<TableMappingFieldNameDto>> GetTableMappingFieldName(string? plant)
        {
            string query = DynamicFormConfigurationQuery.GetTableMappingFieldName;

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<TableMappingFieldNameDto>(query, new { Plant = plant });
        }
    }
}   