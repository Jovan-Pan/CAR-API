using Contracts.Repository.MasterData;
using Dapper;
using Entities.CAR;
using Entities.MasterData;
using Repository.Query;
using System;
using System.Collections.Generic;
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
        public async Task<IEnumerable<ImmidateActionDto>> InsertNewImmidateAction(string ImmidateName, int plant)
        {
            string query = ImmidateActionQuery.InsertNewImmidateAction;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<ImmidateActionDto>(query, new { ImmidateName = ImmidateName , plant = plant });
        }
        public async Task<IEnumerable<ImmidateActionDto>> UpdateImmidateAction(string ImmidateName, int id)
        {
            string query = ImmidateActionQuery.UpdateImmidateAction;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<ImmidateActionDto>(query, new { ImmidateName = ImmidateName, id = id });

        }
        public async Task<IEnumerable<ImmidateActionDto>> DataDelete(int id)
        {
            string query = ImmidateActionQuery.DataDelete;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<ImmidateActionDto>(query, new { id = id });

        }
        public async Task<IEnumerable<ImmidateActionDto>> DataPermDelete(int id)
        {
            string query = ImmidateActionQuery.DataPermDelete;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<ImmidateActionDto>(query, new { id = id });

        }
        public async Task<IEnumerable<ImmidateActionDto>> DataRecover(int id)
        {
            string query = ImmidateActionQuery.DataRecover;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<ImmidateActionDto>(query, new { id = id });

        }
        public async Task<byte[]> Template()
        {
            string filename = Path.Combine(Directory.GetCurrentDirectory(), "Template", "immidate Action.xlsx");
            return await System.IO.File.ReadAllBytesAsync(filename);
        }

    }
}
