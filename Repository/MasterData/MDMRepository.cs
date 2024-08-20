using Contracts.Repository.MasterData;
using Dapper;
using Entities.MasterData;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Repository.Query;

namespace Repository.MasterData;

internal sealed class MDMRepository(DbContext dbContext) : IMDMRepository
{

    public async Task<IEnumerable<string>> GetPlantListForCRCUSystemByUserId(string userId)
    {
        string query = MDMQuery.GetPlantList;

        await using var conn = dbContext.MDMConnection();
        return await conn.QueryAsync<string>(query,new { userId });
    }

    public async Task<IEnumerable<tGlobalSettingDto>> GetDataGlobalSetting(int plant, string system, string settingID)
    {
        await using var conn = dbContext.MDMConnection();
        await conn.OpenAsync();
        //await using SqlTransaction transaction = conn.BeginTransaction();

        string Processquery = MDMQuery.GetDataGlobalSetting;

        return await conn.QueryAsync<tGlobalSettingDto>(Processquery, new { Plant = plant, System = system, SettingID = settingID }, null, 0);
    }
}