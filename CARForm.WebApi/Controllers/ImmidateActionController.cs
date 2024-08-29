using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ImmidateActionController(IServiceManager business) : Controller
    {
        [HttpGet(nameof(getImmidateActionList))]
        public async Task<IActionResult> getImmidateActionList(int plant)
        {
            var result = await business.ImmAct.getImmidateActionList(plant);
            return Ok(result);

        }
    }
}
