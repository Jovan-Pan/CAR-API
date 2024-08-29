using Entities.ParamRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
using Services.Contracts;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class IssueSubmissionController(IServiceManager business) : Controller
    {

        [HttpPost(nameof(ProcessSubmit))]
        public async Task<IActionResult> ProcessSubmit([FromForm] IssueSubmissionParameters data)
        {
            var result = await business.IssueSubmission.ProcessSubmit(data);
            return Ok(result);
        }

        [HttpPost(nameof(issuerUpdate))]
        public async Task<IActionResult> issuerUpdate([FromForm] IssueSubmissionParameters data)
        {
            var result = await business.IssueSubmission.issuerUpdate(data);
            return Ok(result);
        }

        [HttpPost(nameof(issuerMngUpdate))]
        public async Task<IActionResult> issuerMngUpdate([FromForm] IssueSubmissionParameters data)
        {
            var result = await business.IssueSubmission.issuerMngUpdate(data);
            return Ok(result);
        }

        [HttpPost(nameof(pdaDecision))]
        public async Task<IActionResult> pdaDecision([FromForm] IssueSubmissionParameters data)
        {
            var result = await business.IssueSubmission.pdaDecision(data);
            return Ok(result);
        }

        [HttpPost(nameof(pdaDecisionUpdate))]
        public async Task<IActionResult> pdaDecisionUpdate([FromForm] IssueSubmissionParameters data)
        {
            var result = await business.IssueSubmission.pdaDecisionUpdate(data);
            return Ok(result);
        }

        [HttpPost(nameof(pdaApproval))]
        public async Task<IActionResult> pdaApproval([FromForm] IssueSubmissionParameters data)
        {
            var result = await business.IssueSubmission.pdaApproval(data);
            return Ok(result);
        }

        [HttpPost(nameof(ReceiverAction))]
        public async Task<IActionResult> ReceiverAction([FromForm] IssueSubmissionParameters data)
        {
            var result = await business.IssueSubmission.ReceiverAction(data);
            return Ok(result);
        }
    }
}
