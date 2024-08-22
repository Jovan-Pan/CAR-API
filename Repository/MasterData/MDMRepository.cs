using Contracts.Repository.MasterData;
using Dapper;
using Entities.MasterData;
using Entities.ParamRequest;
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
        string Processquery = MDMQuery.GetDataGlobalSetting;

        await using var conn = dbContext.MDMConnection();
        return await conn.QueryAsync<tGlobalSettingDto>(Processquery, new { Plant = plant, System = system, SettingID = settingID });
    }

    public async Task<IEnumerable<TproductVsSmnProdPICDto>> GetTPRODUCT(int plant, string Userid)
    {
        string Processquery = MDMQuery.GetTPRODUCT;

        await using var conn = dbContext.MDMConnection();
        return await conn.QueryAsync<TproductVsSmnProdPICDto>(Processquery, new { plant = plant, Userid = Userid });
    }

    public async Task<IEnumerable<MatGroupDto>> GetMatGrp(int plant, string product, IEnumerable<string> productAuthList)
    {
        string Processquery = MDMQuery.GetMatGrp;

        await using var conn = dbContext.MDMConnection();
        return await conn.QueryAsync<MatGroupDto>(Processquery, new { plant = plant, product = product, productAuthList = productAuthList });
    }

    public async Task<IEnumerable<TMATERIALTYPEDto>> GetMatType(int plant)
    {
        string Processquery = MDMQuery.GetMatType;

        await using var conn = dbContext.MDMConnection();
        return await conn.QueryAsync<TMATERIALTYPEDto>(Processquery, new { plant = plant });
    }

    public async Task<IEnumerable<TMATERIALDto>> GetMaterial(GetMaterialParam request)
    {
        try
        {
            string Processquery = MDMQuery.GetMaterial;

            await using var conn = dbContext.MDMConnection();
            return await conn.QueryAsync<TMATERIALDto>(Processquery, request);
        }
        catch (Exception ex)
        {

            throw;
        }
        
    }
}