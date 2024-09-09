using Entities.ParamRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SendMailSettingController(IServiceManager business) : Controller
    {
        [HttpPost(nameof(GetDataReport))]
        public async Task<IActionResult> GetDataReport([FromBody] SendMailSettingParam data)
        {
            var result = await business.sendmailsetting.GetaData(data);
            return Ok(result);
        }

        [HttpPost(nameof(sendemail))]
        public async Task<IActionResult> sendemail([FromForm] IssueSubmissionParameters data)
        {
            var result = await business.sendmailsetting.sendemail(data);
            return Ok(result);
        }
    }
}
