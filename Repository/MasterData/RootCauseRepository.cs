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
    internal sealed class RootCauseRepository(DbContext dbContext): IRootCauseRepository
    {
        public async Task<IEnumerable<string>> getRootCauseList(int plant)
        {
            string query = RootCauseQuery.getRootCauseList;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<string>(query, new { plant = plant });
        }
    }
}
