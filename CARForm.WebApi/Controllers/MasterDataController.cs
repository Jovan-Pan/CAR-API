using Entities.ParamRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace WebApi.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class MasterDataController(IServiceManager business) : Controller
{
    [HttpGet("GetMenuSetting/{userId}")]
    public async Task<IActionResult> GetMenuSetting(string userId)
    {
        var result = await business.MasterData.GetMenuSetting(userId);
        return Ok(result);
    }
    [AllowAnonymous]
    [HttpGet(nameof(GetPlantListForCRCUSystemByUserId))]
    public async Task<IActionResult> GetPlantListForCRCUSystemByUserId(string userId)
    {
        var plantList = await business.MasterData.GetPlantListForCRCUSystemByUserId(userId);
        return Ok(plantList);
    }

    [HttpGet(nameof(GetDataGlobalSetting))]
    public async Task<IActionResult> GetDataGlobalSetting(int plant, string settingID, string system = "CAR")
    {
        var result = await business.MasterData.GetDataGlobalSetting(plant, system, settingID);
        return Ok(result);

    }

    [HttpGet(nameof(GetTPRODUCT))]
    public async Task<IActionResult> GetTPRODUCT(int plant, string Userid)
    {
        var result = await business.MasterData.GetTPRODUCT(plant, Userid);
        return Ok(result);

    }

    [HttpPost(nameof(GetMatGrp))]
    public async Task<IActionResult> GetMatGrp([FromForm] GetMatGrpParam request)
    {
        var result = await business.MasterData.GetMatGrp(request.plant, request.product, request.productAuthList);
        return Ok(result);

    }

    [HttpGet(nameof(GetMatType))]
    public async Task<IActionResult> GetMatType(int plant)
    {
        var result = await business.MasterData.GetMatType(plant);
        return Ok(result);

    }

    [HttpPost(nameof(getMaterial))]
    public async Task<IActionResult> getMaterial([FromForm] GetMaterialParam request)
    {
        var result = await business.MasterData.GetMaterial(request);
        return Ok(result);

    }

    [HttpGet(nameof(GetNCCategory))]
    public async Task<IActionResult> GetNCCategory()
    {
        var result = await business.MasterData.GetNCCategory();
        return Ok(result);

    }

    [HttpGet(nameof(GetSystemDeptVsUser))]
    public async Task<IActionResult> GetSystemDeptVsUser(int plant, string Userid)
    {
        var result = await business.MasterData.GetSystemDeptVsUser(plant, Userid);
        return Ok(result);

    }

    [HttpGet(nameof(GetVendor))]
    public async Task<IActionResult> GetVendor(int plant)
    {
        var result = await business.MasterData.GetVendor(plant);
        return Ok(result);

    }

    [HttpGet(nameof(getUserFormAuthorize))]
    public async Task<IActionResult> getUserFormAuthorize(int plant, string Userid, string FormName)
    {
        var result = await business.MasterData.GetVendor(plant);
        return Ok(result);

    }

    [HttpGet(nameof(getUserStatusAuthorize))]
    public async Task<IActionResult> getUserStatusAuthorize(int plant, string Userid)
    {
        var result = await business.MasterData.getUserStatusAuthorize(plant, Userid);
        return Ok(result);

    }

    [HttpGet(nameof(GetCurrency))]
    public async Task<IActionResult> GetCurrency(int plant)
    {
        var result = await business.MasterData.GetCurrency(plant);
        return Ok(result);

    }

    [HttpGet(nameof(getProcessGrp))]
    public async Task<IActionResult> getProcessGrp(int plant)
    {
        var result = await business.MasterData.getProcessGrp(plant);
        return Ok(result);

    }

    [HttpGet(nameof(getReason))]
    public async Task<IActionResult> getReason(int plant, string reasontype)
    {
        var result = await business.MasterData.getReason(plant, reasontype);
        return Ok(result);
    }
}
