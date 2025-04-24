using Entities.CAR;
using Entities.MasterData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WorkFlowHistoryController(IServiceManager business) : Controller
    {
        [HttpGet(nameof(GetWorkFlowHistory))]
        [AllowAnonymous]
        public async Task<IActionResult> GetWorkFlowHistory([FromQuery] GETWorkFlowHistory GETWorkFlowHistory)
        {
            var result = await business.WorkFlowHistory.GetWorkFlowHistory(GETWorkFlowHistory);
            return Ok(result);

        }
    }
}
