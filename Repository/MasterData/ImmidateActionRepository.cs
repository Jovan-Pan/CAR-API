using Contracts.Repository.MasterData;
using Dapper;
using Entities.CAR;
using Entities.MasterData;
using Microsoft.Data.SqlClient;
using Repository.Query;
using Services.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.MasterData
{
    internal sealed class ImmidateActionRepository(DbContext dbContext): IImmidateActionRepository
    {
        public async Task<IEnumerable<string>> getImmidateActionList(int plant)
        {
            string query = ImmidateActionQuery.getImmidateActionList;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<string>(query, new { plant = plant });
        }
        public async Task<IEnumerable<ImmidateActionDto>> GetImmidateAction(string? search, string? SearchADV)
        {
            string query;

            if (string.IsNullOrEmpty(search) && string.IsNullOrEmpty(SearchADV))
            {
                query = ImmidateActionQuery.GetImmidateAction;
            }
            else if (!string.IsNullOrEmpty(SearchADV))
            {
                query = ImmidateActionQuery.SearchadvData;
            }
            else
            {
                query = ImmidateActionQuery.SearchDatainDB;
            }
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<ImmidateActionDto>(query, new { search = search, SearchADV = SearchADV });
        }
        public async Task<IEnumerable<ImmidateActionDto>> InsertNewImmidateAction(string ImmidateName, int plant, string userId)
        {
            string query = ImmidateActionQuery.InsertNewImmidateAction;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<ImmidateActionDto>(query, new { ImmidateName = ImmidateName , plant = plant , userId = userId });
        }
        public async Task<IEnumerable<ImmidateActionDto>> UpdateImmidateAction(string ImmidateName, int id, string userId)
        {
            string query = ImmidateActionQuery.UpdateImmidateAction;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<ImmidateActionDto>(query, new { ImmidateName = ImmidateName, id = id, userId = userId });

        }
        public async Task<IEnumerable<ImmidateActionDto>> DataDelete(int id, string userId)
        {
            string query = ImmidateActionQuery.DataDelete;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<ImmidateActionDto>(query, new { id = id , userId = userId });

        }
        public async Task<IEnumerable<ImmidateActionDto>> DataPermDelete(int id)
        {
            string query = ImmidateActionQuery.DataPermDelete;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<ImmidateActionDto>(query, new { id = id });

        }
        public async Task<IEnumerable<ImmidateActionDto>> DataRecover(int id, string userId)
        {
            string query = ImmidateActionQuery.DataRecover;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<ImmidateActionDto>(query, new { id = id , userId = userId});

        }
        public async Task<byte[]> Template()
        {
            string filename = AppDomain.CurrentDomain.BaseDirectory + "Template\\immidate Action.xlsx";
            return await System.IO.File.ReadAllBytesAsync(filename);
        }
        public async Task<IEnumerable<string>> Import(string filePath, string userId)
        {
            string excelCol = "[Plant], [Immidate Name]";
            string excelRange = "A4:E5000";
            string query = ImmidateActionQuery.Import;
            ArrayList conditions = new ArrayList();
            ArrayList condRemark = new ArrayList();
            ArrayList specialCond = new ArrayList();
            conditions.Add(" ISNULL([Immidate Name], '') = ''  or ISNULL(Plant, '') = ''  ");
            condRemark.Add("Null Mandatory Data");
            conditions.Add(" LEN([Immidate Name]) > 50");
            condRemark.Add("Process Group Code maximal 50 characters");

            string uniqueField = "[Immidate Name]";

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
            if (!excelData.Columns.Contains("Plant") || !excelData.Columns.Contains("Immidate Name"))
            {
                return new List<string> { "Invalid Data structure, please follow template format" };
            }

            using (var transaction = conn.BeginTransaction())
            {
                // Membuat temp table
                var createTempTable = @"CREATE TABLE ##temp (
                                Plant NVARCHAR(50),
                                [Immidate Name] NVARCHAR(50)
                            )";
                await conn.ExecuteAsync(createTempTable, transaction: transaction);

                using (var bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                {
                    bulkCopy.DestinationTableName = "##temp"; // Temp table
                    bulkCopy.ColumnMappings.Add("Plant", "Plant");
                    bulkCopy.ColumnMappings.Add("Immidate Name", "Immidate Name");
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
