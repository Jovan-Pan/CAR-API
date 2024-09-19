using Entities.ParamRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorLogController (IServiceManager business) : Controller
    {

        //[HttpPost(nameof(GetDataReport))]
        //public async Task<IActionResult> GetDataReport([FromBody] xxxxxParam data)
        //{
        //    var result = await business.ErrorLog.GetDataReport(data);
        //    return Ok(result);
        //}

        [AllowAnonymous]
        [HttpPost(nameof(GetDataReport))]
        public async Task<IActionResult> GetDataReport(string? startdate, string? enddate)
        {
            var result = await business.ErrorLog.GetDataReport(startdate, enddate);
            return Ok(result);
        }
    }
}
