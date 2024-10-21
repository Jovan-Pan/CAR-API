using Contracts.Repository.CAR;
using Dapper;
using Entities.CAR;
using Entities.ParamRequest;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
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

namespace Repository.CAR
{
    internal sealed class SendMailSettingRepository(DbContext dbContext): ISendMailSettingRepository
    {
        public async Task<IEnumerable<MailSetiingDto>> GetaData(SendMailSettingParam param)
        {
            string query = string.Format(SendMailSettingQuery.GetaData);

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<MailSetiingDto>(query, param);

        }

        public async Task<IEnumerable<MailSetiingDto>> GetSendMailSetting(string? search, string? ATsearchADV, string? ATDsearchADV)
        {

            string query;

            if (string.IsNullOrEmpty(search) && string.IsNullOrEmpty(ATsearchADV) && string.IsNullOrEmpty(ATDsearchADV))
            {
                query = SendMailSettingQuery.GetSendMailSetting;
            }
            else if(!string.IsNullOrEmpty(ATsearchADV) || !string.IsNullOrEmpty(ATDsearchADV))
            {
                query = SendMailSettingQuery.SearchadvData;
            }
            else
            {
                query = SendMailSettingQuery.SearchDatainDB;
            }
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<MailSetiingDto>(query, new { search = search, ATsearchADV = ATsearchADV, ATDsearchADV = ATDsearchADV });

        }
        public async Task<IEnumerable<MailSetiingDto>> InsertNewSendMailSetting(string plant, string actiontype, string actiontypedesc, bool issendemail, string userId)
        {
            string query = string.Format(SendMailSettingQuery.InsertNewSendMailSetting);

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<MailSetiingDto>(query,new {plant = plant, actiontype = actiontype, actiontypedesc = actiontypedesc, issendemail = issendemail, userId = userId });

        }

        public async Task<IEnumerable<MailSetiingDto>> UpdateSendMailSetting(string actiontype, string actiontypedesc, bool issendemail, string userId)
        {
            string query = string.Format(SendMailSettingQuery.UpdateSendMailSetting);

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<MailSetiingDto>(query, new { actiontype = actiontype, actiontypedesc = actiontypedesc, issendemail = issendemail , userId =  userId});

        }

        public async Task<IEnumerable<MailSetiingDto>> DataDelete(string actiontype, string userId)
        {
            string query = string.Format(SendMailSettingQuery.DataDelete);

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<MailSetiingDto>(query, new { actiontype = actiontype, userId = userId});

        }

        public async Task<IEnumerable<MailSetiingDto>> DataPermDelete(string actiontype)
        {
            string query = string.Format(SendMailSettingQuery.DataPermDelete);

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<MailSetiingDto>(query, new { actiontype = actiontype });

        }

        public async Task<IEnumerable<MailSetiingDto>> DataRecover(string actiontype, string userId)
        {
            string query = string.Format(SendMailSettingQuery.DataRecover);

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<MailSetiingDto>(query, new { actiontype = actiontype, userId = userId });

        }
        public async Task<byte[]> Template()
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "\\Excel";
            string fileName = string.Format("immidate_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss"));
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
            var row2Header = new object[] { "Mandatory", "Mandatory", "Mandatory", "Mandatory" };
            var row3Header = new object[] { "int", "nvarchar(50)", "nvarchar(150)", "Bit(Y/N)" };
            var row4Header = new object[] { "Plant", "ActionType", "ActionTypeDesc", "IsSendEmail" };
            var row5Header = new object[] { "2100", "Test", "Test", "Y" };

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
            using (var range = sheet.Cells[1, 1, 3, 5])
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
            string excelCol = "[Plant], [Action Type], [Action Type Desc], isSendEmail";
            string excelRange = "A4:E5000";
            string query = SendMailSettingQuery.Import;
            ArrayList conditions = new ArrayList();
            ArrayList condRemark = new ArrayList();
            ArrayList specialCond = new ArrayList();
            conditions.Add(" ISNULL([Action Type], '') = '' or ISNULL(Plant, '') = ''  ");
            condRemark.Add("Null Mandatory Data");
            conditions.Add(" LEN([Action Type]) > 50");
            condRemark.Add("Action Type maximal 50 characters");

            string uniqueField = "[Action Type]";

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

            if (!excelData.Columns.Contains("Plant") || !excelData.Columns.Contains("Action Type") || !excelData.Columns.Contains("Action Type Desc"))
            {
                return new List<string> { "Invalid Data structure, please follow template format" };
            }

            using (var transaction = conn.BeginTransaction())
            {
                // Membuat temp table
                var createTempTable = @"CREATE TABLE ##temp (
                                Plant int,
                                [Action Type] NVARCHAR(50),
                                [Action Type Desc] NVARCHAR(150),
                                isSendEmail bit
                            )";
                await conn.ExecuteAsync(createTempTable, transaction: transaction);

                foreach (DataRow row in excelData.Rows)
                {
                    if (excelData.Columns.Contains("isSendEmail"))
                    {
                        // Convert '0' or '1' strings to boolean values (bit in SQL)
                        row["isSendEmail"] = row["isSendEmail"].ToString() == "Y" ? true : false;
                    }
                }
                using (var bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkCopy.DestinationTableName = "##temp"; // Temp table
                    bulkCopy.ColumnMappings.Add("Plant", "Plant");
                    bulkCopy.ColumnMappings.Add("Action Type", "Action Type");
                    bulkCopy.ColumnMappings.Add("Action Type Desc", "Action Type Desc");
                    bulkCopy.ColumnMappings.Add("isSendEmail", "isSendEmail");
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
        public async Task<byte[]> Export(ExportParam param)
        {
            StringBuilder queryBuilder = new StringBuilder(SendMailSettingQuery.Export);

            if (param.plant.HasValue)
            {
                queryBuilder.Append(" AND CAST(plant AS VARCHAR) LIKE @plant");
            }

            if (!string.IsNullOrEmpty(param.actionType))
            {
                queryBuilder.Append(" AND actionType LIKE @actionType");
            }

            if (!string.IsNullOrEmpty(param.actionTypeDesc))
            {
                queryBuilder.Append(" AND actionTypeDesc LIKE @actionTypeDesc");
            }

            if (!string.IsNullOrEmpty(param.CreatedBy))
            {
                queryBuilder.Append(" AND CreatedBy LIKE @CreatedBy");
            }

            if (!string.IsNullOrEmpty(param.CreatedByName))
            {
                queryBuilder.Append(" AND CreatedByName LIKE @CreatedByName");
            }

            if (!string.IsNullOrEmpty(param.updatedBy))
            {
                queryBuilder.Append(" AND updatedBy LIKE @updatedBy");
            }

            if (!string.IsNullOrEmpty(param.updatedByName))
            {
                queryBuilder.Append(" AND updatedByName LIKE @updatedByName");
            }
            if (!string.IsNullOrEmpty(param.CreatedDateInput))
            {
                queryBuilder.Append(" AND FORMAT(CreatedDate, 'dd-MM-yyyy HH:mm:ss.SSS') LIKE @CreatedDate");
            }

            // Handle updatedDate search with the full 'dd-MM-yyyy HH:mm:ss.SSS' format
            if (!string.IsNullOrEmpty(param.updatedDateInput))
            {
                queryBuilder.Append(" AND FORMAT(updatedDate, 'dd-MM-yyyy HH:mm:ss.SSS') LIKE @updatedDate");
            }

            // Ensure OR clauses are grouped correctly
            if (!string.IsNullOrEmpty(param.globalSearch))
            {
                queryBuilder.Append(" AND (plant LIKE @globalSearch OR actionType LIKE @globalSearch OR actionTypeDesc LIKE @globalSearch OR CreatedBy LIKE @globalSearch OR CreatedByName LIKE @globalSearch OR FORMAT(CreatedDate, 'dd-MM-yyyy HH:mm:ss') LIKE @globalSearch OR updatedBy LIKE @globalSearch OR updatedByName LIKE @globalSearch OR FORMAT(updatedDate, 'dd-MM-yyyy HH:mm:ss') LIKE @globalSearch)");
            }

            if (!string.IsNullOrEmpty(param.search))
            {
                queryBuilder.Append(" AND (actionType LIKE @search OR actionTypeDesc LIKE @search)");
            }

            if (!string.IsNullOrEmpty(param.ATsearchADV) || !string.IsNullOrEmpty(param.ATDsearchADV))
            {
                queryBuilder.Append(" AND (actionType LIKE @ATsearchADV OR actionTypeDesc LIKE @ATDsearchADV)");
            }

            string query = queryBuilder.ToString();

            var parameters = new
            {
                plant = param.plant.HasValue ? $"%{param.plant}%" : null,
                actionType = !string.IsNullOrEmpty(param.actionType) ? $"%{param.actionType}%" : null,
                actionTypeDesc = !string.IsNullOrEmpty(param.actionTypeDesc) ? $"%{param.actionTypeDesc}%" : null,
                CreatedBy = !string.IsNullOrEmpty(param.CreatedBy) ? $"%{param.CreatedBy}%" : null,
                CreatedByName = !string.IsNullOrEmpty(param.CreatedByName) ? $"%{param.CreatedByName}%" : null,
                updatedBy = !string.IsNullOrEmpty(param.updatedBy) ? $"%{param.updatedBy}%" : null,
                updatedByName = !string.IsNullOrEmpty(param.updatedByName) ? $"%{param.updatedByName}%" : null,
                CreatedDate = !string.IsNullOrEmpty(param.CreatedDateInput) ? $"%{param.CreatedDateInput}%" : null,
                updatedDate = !string.IsNullOrEmpty(param.updatedDateInput) ? $"%{param.updatedDateInput}%" : null,
                globalSearch = !string.IsNullOrEmpty(param.globalSearch) ? $"%{param.globalSearch}%" : null,
                search = !string.IsNullOrEmpty(param.search) ? $"%{param.search}%" : null,
                ATsearchADV = !string.IsNullOrEmpty(param.ATsearchADV) ? $"%{param.ATsearchADV}%" : null,
                ATDsearchADV = !string.IsNullOrEmpty(param.ATDsearchADV) ? $"%{param.ATDsearchADV}%" : null
            };

            await using var conn = dbContext.CARConnection();
            var data = (await conn.QueryAsync<ExportParam>(query, parameters)).ToList();

            var projectedData = data.Select(d => new
            {
                d.plant,
                d.actionType,
                d.actionTypeDesc,
                d.isSendEmail,
                d.CreatedBy,
                d.CreatedByName,
                d.CreatedDate,
                d.updatedBy,
                d.updatedByName,
                d.updatedDate,
                d.isDeleted
            }).ToList();

            using (var package = new ExcelPackage())
            {
                // Create a worksheet
                var worksheet = package.Workbook.Worksheets.Add("MailSettings");

                // Load data into Excel (assuming you have a property list to insert in the Excel)
                worksheet.Cells["A1"].LoadFromCollection(projectedData, true);

                //if (projectedData.Any())
                //{
                //    var firstItem = projectedData.First();
                //    PropertyInfo[] properties = firstItem.GetType().GetProperties();

                //    // Loop through properties to identify DateTime columns
                //    for (int i = 0; i < properties.Length; i++)
                //    {
                //        var property = properties[i];

                //        // Check if the property is DateTime or DateTime?
                //        if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                //        {
                //            // Apply custom date formatting for columns with DateTime type
                //            worksheet.Column(i + 1).Style.Numberformat.Format = "dd-MM-yyyy HH:mm:ss.SSS";
                //        }
                //        else if (property.PropertyType == typeof(double)) // Check for Excel serial numbers
                //        {
                //            // Optionally, check if the double value represents a valid Excel DateTime serial number
                //            if (projectedData.Any(d => ((double?)property.GetValue(d) ?? 0) > 0))
                //            {
                //                // Apply date formatting to serial number columns
                //                worksheet.Column(i + 1).Style.Numberformat.Format = "dd-MM-yyyy HH:mm:ss.SSS";
                //            }
                //        }
                //    }
                //}

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Return the Excel file as a byte array
                return package.GetAsByteArray();
            }
        }
    }
}
