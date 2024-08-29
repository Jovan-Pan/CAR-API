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
        request.MaterialList = null;
        string Processquery = MDMQuery.GetMaterial;

        await using var conn = dbContext.MDMConnection();
        return await conn.QueryAsync<TMATERIALDto>(Processquery, request);
    }

    public async Task<IEnumerable<TMATERIALDto>> GetMaterialWoProdAut(GetMaterialParam request)
    {
        string Processquery = MDMQuery.GetMaterialWoProdAut;

        await using var conn = dbContext.MDMConnection();
        return await conn.QueryAsync<TMATERIALDto>(Processquery, request);
    }

    public async Task<IEnumerable<NcCategoryDto>> GetNCCategory()
    {
        string Processquery = MDMQuery.GetNCCategory;
        await using var conn = dbContext.MDMConnection();
        return await conn.QueryAsync<NcCategoryDto>(Processquery);
    }

    public async Task<IEnumerable<SystemDeptVsUserDto>> GetSystemDeptVsUser(int plant, string Userid)
    {
        string Processquery = MDMQuery.GetSystemDeptVsUser;
        await using var conn = dbContext.MDMConnection();
        return await conn.QueryAsync<SystemDeptVsUserDto>(Processquery, new { plant = plant, Userid = Userid });
    }

    public async Task<IEnumerable<VendorDto>> GetVendor(int plant)
    {
        string Processquery = MDMQuery.GetVendor;
        await using var conn = dbContext.MDMConnection();
        return await conn.QueryAsync<VendorDto>(Processquery, new { plant = plant });
    }

    public async Task<IEnumerable<BasePathConfigDto>> getBasePathConfig(int plant)
    {
        string Processquery = MDMQuery.getBasePathConfig;
        await using var conn = dbContext.MDMConnection();
        return await conn.QueryAsync<BasePathConfigDto>(Processquery, new { plant = plant });
    }

    public async Task<IEnumerable<UserFormAuthorizeDto>> getUserFormAuthorize(int plant, string Userid, string FormName)
    {
        string Processquery = MDMQuery.getUserFormAuthorize;
        await using var conn = dbContext.MDMConnection();
        return await conn.QueryAsync<UserFormAuthorizeDto>(Processquery, new { plant = plant, UserId= Userid, FormName= FormName, });
    }

    public async Task<IEnumerable<string>> getUserStatusAuthorize(int plant, string Userid)
    {
        string query = MDMQuery.getUserStatusAuthorize;
        await using var conn = dbContext.MDMConnection();
        return await conn.QueryAsync<string>(query, new { plant = plant, UserId = Userid});
    }

    public async Task<IEnumerable<CurrencyDto>> GetCurrency(int plant)
    {
        string Processquery = MDMQuery.GetCurrency;
        await using var conn = dbContext.MDMConnection();
        return await conn.QueryAsync<CurrencyDto>(Processquery, new { plant = plant });
    }

    public async Task<IEnumerable<ProcessGroupDto>> getProcessGrp(int plant)
    {
        string Processquery = MDMQuery.getProcessGrp;
        await using var conn = dbContext.MDMConnection();
        return await conn.QueryAsync<ProcessGroupDto>(Processquery, new { plant = plant });
    }
}