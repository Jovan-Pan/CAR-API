using Contracts.Repository.MasterData;
using Dapper;
using Entities.Account.Dto;
using Entities.CAR;
using Entities.MasterData;
using Entities.ParamRequest;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Repository.Query;
using System.Net.NetworkInformation;

namespace Repository.MasterData;

internal sealed class MDMRepository(DbContext dbContext) : IMDMRepository
{
    public async Task<bool> AllowAllDataToAcc(string userId)
    {
        string query = MDMQuery.AllowAllDataToAcc;

        await using var conn = dbContext.MDMConnection();
        var result = await conn.ExecuteScalarAsync<bool>(query, new { userId });
        return result;
    }
    public async Task<IEnumerable<string>> GetPlantListForCRCUSystemByUserId(string userId)
    {
        string query = MDMQuery.GetPlantList;

        await using var conn = dbContext.MDMConnection();
        return await conn.QueryAsync<string>(query,new { userId });
    }

    public async Task<FormAuthorizeInfoDto> GetFormAuthorize(FormAuthorizeParam param)
    {
        string query = MDMQuery.GetFormAuthorize;

        await using var conn = dbContext.MDMConnection();
        return await conn.QueryFirstOrDefaultAsync<FormAuthorizeInfoDto>(query, param);
    }

    public async Task<UserVendorInfoDto> GetUserVendorInfo(int plant, string userid)
    {
        string Processquery = MDMQuery.GetUserVendorInfo;

        await using var conn = dbContext.MDMConnection();
        var result = await conn.QueryAsync<UserVendorInfoDto>(Processquery, new { Plant = plant, userid = userid });
        return result.FirstOrDefault();
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
    public async Task<IEnumerable<SystemDeptVsUserDto>> GetSystemDeptVsUserDynamic(int plant, string Userid)
    {
        string Processquery = MDMQuery.GetSystemDeptVsUserDynamic;
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

    public async Task<IEnumerable<string>> getReason(int plant, string reasontype)
    {
        string query = MDMQuery.getReason;
        await using var conn = dbContext.MDMConnection();
        return await conn.QueryAsync<string>(query, new { plant = plant, reasontype = reasontype });
    }

    public async Task<IEnumerable<TGlobalEmailSettingModel>> GetTGlobalEmailSetting(int plant, string wStatus)
    {
        string Processquery = MDMQuery.GetTGlobalEmailSetting;
        await using var conn = dbContext.MDMConnection();
        return await conn.QueryAsync<TGlobalEmailSettingModel>(Processquery, new { plant, wStatus });
    }

    public async Task<IEnumerable<SystemvsUservsEmailSubscribeForm>> GetSystemvsUservsEmailSubscribeForm(int plant, string group, string dept)
    {
        string Processquery = MDMQuery.GetSystemvsUservsEmailSubscribeForm;
        await using var conn = dbContext.MDMConnection();
        return await conn.QueryAsync<SystemvsUservsEmailSubscribeForm>(Processquery, new { plant, group, dept });
    }
    public async Task<IEnumerable<SystemvsUservsEmailSubscribeForm>> GetSystemvsUservsEmailSubscribeFormVendor(string VendorCode, int plant, string group, string dept)
    {
        string Processquery = MDMQuery.GetSystemvsUservsEmailSubscribeFormVendor;
        await using var conn = dbContext.MDMConnection();
        return await conn.QueryAsync<SystemvsUservsEmailSubscribeForm>(Processquery, new { VendorCode, plant, group, dept });
    }

    public async Task<IEnumerable<string>> GetissuerEmail(int plant, IEnumerable<string> UseID)
    {
        string Processquery = MDMQuery.GetissuerEmail;
        await using var conn = dbContext.MDMConnection();
        return await conn.QueryAsync<string>(Processquery, new { plant,UseID });
    }

    public async Task<bool> GetisSpAdmin(int plant, string UseID)
    {
        string Processquery = MDMQuery.GetisSpAdmin;
        await using var conn = dbContext.MDMConnection();
        return await conn.QueryFirstOrDefaultAsync<bool>(Processquery, new { plant, UseID });
    }
    public async Task<IEnumerable<UsrDto>> GetUsr(DynamicFormParameterDTO data)
    {
        string Processquery;
        if (string.IsNullOrEmpty(data.VendorCode) && string.IsNullOrEmpty(data.Dept))
        {
            Processquery = MDMQuery.GetUsr;
        }
        //else if(!string.IsNullOrEmpty(data.VendorCode) && data.Dept == "VEND")
        //{
        //    Processquery = MDMQuery.GetUsrForVndr;
        //}
        else
        {
            Processquery = MDMQuery.GetUsrForDept;
        }
        await using var conn = dbContext.MDMConnection();
        return await conn.QueryAsync<UsrDto>(Processquery, new {Vendor = data.VendorCode, Dept = data.Dept,plant =data.UserPlant } );
    }
    public async Task<IEnumerable<UsrDto>> CheckUserVSVend(DynamicFormParameterDTO data)
    { 
        string query = MDMQuery.CheckUserVSVend;
        await using var conn = dbContext.MDMConnection();
        var result = await conn.QueryAsync<UsrDto>(query, data);
        return result ?? Enumerable.Empty<UsrDto>();
    }
    public async Task<IEnumerable<PlantDto>> GetPlant(int plant)
    {
        string query = MDMQuery.getplant;
        await using var conn = dbContext.MDMConnection();
        var result = await conn.QueryAsync<PlantDto>(query, new { plant = plant });
        return result ?? Enumerable.Empty<PlantDto>();
    }
}
