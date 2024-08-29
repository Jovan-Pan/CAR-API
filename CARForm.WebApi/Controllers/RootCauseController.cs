using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RootCauseController(IServiceManager business) : Controller
    {
        [HttpGet(nameof(getRootCauseList))]
        public async Task<IActionResult> getRootCauseList(int plant)
        {
            var result = await business.RootCause.getRootCauseList(plant);
            return Ok(result);

        }
    }
}
