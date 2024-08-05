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
}
