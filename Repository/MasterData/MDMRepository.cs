using Contracts.Repository.MasterData;
using Dapper;

namespace Repository.MasterData;

internal sealed class MDMRepository(DbContext dbContext) : IMDMRepository
{

    public async Task<IEnumerable<string>> GetPlantListForCRCUSystemByUserId(string userId)
    {
        const string query =
            @"Select 
	                Plant 
                FROM 
	                TGROUP g
	                INNER JOIN TUSER_AUTHORIZE ua on (g.GroupID = ua.GroupID)
                WHERE
	                g.System = 'CRCU'
	                AND ua.UserId = @userId
                GROUP BY 
	                Plant";

        await using var conn = dbContext.MDMConnection();
        return await conn.QueryAsync<string>(query,
            new { userId });
    }
}