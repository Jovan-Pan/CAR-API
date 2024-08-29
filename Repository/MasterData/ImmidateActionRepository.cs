using Contracts.Repository.MasterData;
using Dapper;
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
    }
}
