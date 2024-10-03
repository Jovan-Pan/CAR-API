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

namespace Repository.MasterData
{
    internal sealed class RootCauseRepository(DbContext dbContext): IRootCauseRepository
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
            string filename = Path.Combine(Directory.GetCurrentDirectory(), "Template","Root Cause Category.xlsx");
            return await System.IO.File.ReadAllBytesAsync(filename);
        }
        public async Task<IEnumerable<string>> Import(string filePath, string userId)
        {
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
