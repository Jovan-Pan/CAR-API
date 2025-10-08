using Contracts.Repository.CAR;
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

namespace Repository.CAR
{
    internal sealed class DraftCARRepository(DbContext dbContext) : IDraftCARRepository
    {
        public async Task<byte[]> Template()
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "\\Excel";
            string fileName = string.Format("DynamicFlowConfiguration_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss"));
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
            var row2Header = new object[] { "Mandatory", "Mandatory", "", "Mandatory", "", "Mandatory", "Mandatory", "Mandatory", "", "", "Mandatory", "Mandatory", "", "", "Mandatory", "" };
            var row3Header = new object[] { "int", "nvarchar(20)", "nvarchar(4)", "nvarchar(40)", "nvarchar(100)", "nvarchar(10)", "int", "nvarchar(6)", "nvarchar(8)", "nvarchar(100)", "int", "int", "nvarchar(20)", "nvarchar(150)", "nvarchar(100)", "int" };
            var row4Header = new object[] { "Plant", "FormType", "Product", "Material Code", "Material Description", "UOM", "Total Qty", "Supplier Dept", "Supplier Vendor", "Supplier Name", "Inspected Sample", "Nonconforming", "NC Category", "NC Description", "Status of finding", "Affected Cavity" };
            var row5Header = new object[] { "2310", "QFR", "", "70230246", "7WHSOA3 G-CARD COA32360", "PC", "37", "QC", "", "", "10", "10", "402", "HUMAN-MIX MODEL", "Non Conformance ", "" };

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

            sheet.Cells[startRow, startColumn, data.Count, row2Header.Length].AutoFitColumns();

            using (var range = sheet.Cells[1, row1Header.Length])
            {
                range.Style.Font.Color.SetColor(Color.Red); // Set the font color to red
            }
            using (var range = sheet.Cells[1, 1, 3, 16])
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
        }

        public async Task<ImportResult> Import(string filePath, string userId, string userName)
        {

            /**Query draft
             string query = IssueSubmissionQuery.SaveAsDraftDataIssueFeedback;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
             **/

            string excelCol = "Plant,FormType,Product,Material Code,Material Description,UOM,Total Qty,Supplier Dept,Supplier Vendor,Supplier Name,Inspected Sample,Nonconforming,NC Category,NC Description,Status of finding,Affected Cavity\r\n";
            string excelRange = "A4:R5000";
            string query = DraftCARQuery.ImportDraftCAR;

            ArrayList conditions = new ArrayList();
            ArrayList condRemark = new ArrayList();
            ArrayList specialCond = new ArrayList();

            conditions.Add("ISNULL(Plant, '') = ''");
            condRemark.Add("Plant Is required");

            conditions.Add("ISNULL(FormType, '') = ''");
            condRemark.Add("Form Type Is required");

            conditions.Add("ISNULL([Supplier Dept], '') = ''");
            condRemark.Add("Please fill Dept!");

            conditions.Add("ISNULL([Material Code], '') = ''");
            condRemark.Add("Please fill Material Code!");

            conditions.Add("ISNULL([UOM], '') = ''");
            condRemark.Add("Please fill UOM!");

            conditions.Add("ISNULL([Total Qty], '')= ''");
            condRemark.Add("Please fill Total Qty!");

            conditions.Add("TRY_CAST([Total Qty] AS FLOAT) IS NULL");
            condRemark.Add("Qty must be numeric!");

            conditions.Add("TRY_CAST([Total Qty] AS FLOAT) < 0");
            condRemark.Add("Qty cannot be negative!");

            conditions.Add("ISNULL([Supplier Dept], '') = ''");
            condRemark.Add("Please fill Supplier Dept!");

            conditions.Add("ISNULL([Inspected Sample], '') = ''");
            condRemark.Add("Please fill Inspected Sample!");

            conditions.Add("TRY_CAST([Inspected Sample] AS FLOAT) IS NULL");
            condRemark.Add("Inspected Sample must be numeric!");

            conditions.Add("TRY_CAST([Inspected Sample] AS FLOAT) < 0");
            condRemark.Add("Inspected Sample cannot be negative!");

            conditions.Add("ISNULL([Nonconforming], '')= ''");
            condRemark.Add("Please fill Nonconforming!");

            conditions.Add("TRY_CAST([Nonconforming] AS FLOAT) IS NULL");
            condRemark.Add("Nonconforming must be numeric!");

            conditions.Add("TRY_CAST([Nonconforming] AS FLOAT) < 0");
            condRemark.Add("Nonconforming cannot be negative!");

            conditions.Add("[Status of finding] != 'Non Conformance'");
            condRemark.Add("Status of finding must be Non Conformance!");

            conditions.Add("[Supplier Dept] != 'VEND' AND [Supplier Vendor] != ''");
            condRemark.Add("Please choose one Dept or Vendor!");

            conditions.Add("TRY_CAST([Affected Cavity] AS FLOAT) IS NULL");
            condRemark.Add("Affected Cavity must be numeric!");

            string uniqueField = "";
            string CheckingMethod = "1";

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
            ExcelReadResponseDto excelData = GlobalFunction.ReadExcelFile(filePath, userId, query, excelCol, "DraftCAR", "DraftCAR", conditions, condRemark, excelRange, uniqueField);

            if (!excelData.Success)
            {
                System.IO.File.Delete(filePath);
                return new ImportResult { Success = false, Message = "Import Failed: " + excelData.Message };
            }

            var requiredColumns = new[] { "Issue Remark", "FormNo", "Status", "CheckingMethod" };
            foreach (var col in requiredColumns)
            {
                if (!excelData.DataTable.Columns.Contains(col))
                {
                    excelData.DataTable.Columns.Add(col, typeof(string));
                }
            }


            System.IO.File.Delete(filePath);

            using (var transaction = conn.BeginTransaction())
            {
                try
                {
                    // Create temp table
                    var createTempTable = @"IF OBJECT_ID('tempdb..##temp_DraftCAR') IS NOT NULL DROP TABLE ##temp_DraftCAR; CREATE TABLE ##temp_DraftCAR (";
                    var columnMappings = new List<string>(); // For SqlBulkCopy mappings

                    // Loop through the columns in the DataTable to create table definition and mappings
                    //foreach (DataColumn column in excelData.DataTable.Columns)
                    //{
                    //    if (!string.IsNullOrWhiteSpace(column.ColumnName))
                    //    {
                    //        createTempTable += $"[{column.ColumnName}] NVARCHAR(MAX) COLLATE DATABASE_DEFAULT, ";
                    //        columnMappings.Add(column.ColumnName);
                    //    }
                    //}

                    string formNo = "";
                    string QueryGetFormNo = IssueSubmissionQuery.GenerateNewFormNo;
                    var addedColumns = new HashSet<string>();

                    foreach (DataRow row in excelData.DataTable.Rows)
                    {

                        //To skip empty row
                        if (IsRowEmpty(row)) continue;

                        string plant = row["Plant"].ToString();
                        string FormType = row["FormType"].ToString();

                        formNo = await conn.QueryFirstOrDefaultAsync<string>(QueryGetFormNo, new { plant = plant, FormType = FormType }, transaction);
                        row["formNo"] = formNo;
                        row["Status"] = "DRAFT-SUBMIT";
                        row["CheckingMethod"] = "1";

                        foreach (DataColumn column in excelData.DataTable.Columns)
                        {
                            string colName = column.ColumnName;

                            if (!string.IsNullOrWhiteSpace(colName) && !addedColumns.Contains(colName))
                            {
                                createTempTable += $"[{colName}] NVARCHAR(MAX) COLLATE DATABASE_DEFAULT, ";
                                columnMappings.Add(colName);
                                addedColumns.Add(colName);
                            }

                            var value = row[column]; // tetap bisa ambil value jika diperlukan
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
                        bulkCopy.DestinationTableName = "##temp_DraftCAR";
                        foreach (var columnName in columnMappings)
                        {
                            bulkCopy.ColumnMappings.Add(columnName, columnName);
                        }
                        await bulkCopy.WriteToServerAsync(excelData.DataTable);
                    }

                    // Trim columns and initialize Issue Remark
                    var columnNameTrim = string.Join(", ", excelData.DataTable.Columns.Cast<DataColumn>()
                        .Select(c => $"[{c.ColumnName}] = LTRIM(RTRIM([{c.ColumnName}]))"));

                    string sql = $@"UPDATE ##temp_DraftCAR SET {columnNameTrim}; UPDATE ##temp_DraftCAR SET [Issue Remark] = '';";

                    // Check for invalid data
                    string formattedCol = string.Join(", ", excelCol.Split(',').Select(col => $"[{col.Trim()}]"));

                    sql += @" IF OBJECT_ID('tempdb..#invaliddata_DraftCAR') IS NOT NULL DROP TABLE #invaliddata_DraftCAR;
                          SELECT TOP 0 * INTO #invaliddata_DraftCAR FROM ##temp_DraftCAR;";
                    if (conditions != null && conditions.Count > 0)
                    {

                        for (int i = 0; i < conditions.Count; i++)
                        {
                            sql += $@"
                            INSERT INTO #invaliddata_DraftCAR ({formattedCol}, [Issue Remark])
                            SELECT {formattedCol}, '{condRemark[i]}'
                            FROM ##temp_DraftCAR
                            WHERE {conditions[i]} AND CheckingMethod IS NOT NULL;

                            DELETE FROM ##temp_DraftCAR WHERE {conditions[i]};";
                        }
                    }

                    //query to check is material exists or not

                    string formattedCol2 = string.Join(",", formattedCol.Split(',').Select(col => $"a.{col}"));

                    sql += $@"
                            INSERT INTO #invaliddata_DraftCAR ({formattedCol2}, [Issue Remark]) 
                            SELECT {formattedCol2}, 'The material is not exists' 
                            FROM ##temp_DraftCAR a LEFT JOIN MDMTmaterial b ON a.Plant=b.Plant AND a.[Material Code]=b.Material 
                            WHERE b.Material IS NULL 
                            ";

                    // Check for duplicate data
                    //if (!string.IsNullOrEmpty(uniqueField))
                    //{
                    //    sql += $@"
                    //    WITH cte AS (
                    //        SELECT {excelCol}, ROW_NUMBER() OVER (PARTITION BY {uniqueField} ORDER BY {uniqueField}) AS row_num
                    //        FROM ##temp
                    //    )
                    //    INSERT INTO #invaliddata ({excelCol}, [Issue Remark])
                    //    SELECT {excelCol}, 'Duplicate Excel Data' FROM cte WHERE row_num > 1;

                    //    WITH cte AS (
                    //        SELECT *,
                    //                ROW_NUMBER() OVER (PARTITION BY {uniqueField} ORDER BY (SELECT NULL)) AS row_num
                    //        FROM ##temp
                    //    )
                    //    DELETE FROM cte WHERE row_num > 1";

                    //}

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
                    var invalidData = await conn.QueryAsync($"SELECT * FROM #invaliddata_DraftCAR", transaction: transaction);

                    List<Dictionary<string, object>> dataListInValid = invalidData
                     .Select(row => new Dictionary<string, object>(row))
                     .ToList();

                    // Count valid data
                    var validDataCount = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM ##temp_DraftCAR", transaction: transaction);

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
                            UserId = userId,
                            UserName = userName,
                            CheckingMethod = CheckingMethod
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

            bool IsRowEmpty(DataRow row)
            {
                foreach (var item in row.ItemArray)
                {
                    if (item != null && !string.IsNullOrWhiteSpace(item.ToString()))
                    {
                        return false;
                    }
                }
                return true;
            }

        }
    }
}
