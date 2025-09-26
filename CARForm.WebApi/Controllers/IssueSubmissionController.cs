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

        [HttpPost(nameof(issuerVoid))]
        public async Task<IActionResult> issuerVoid([FromForm] IssueSubmissionParameters data)
        {
            var result = await business.IssueSubmission.issuerVoid(data);
            return Ok(result);
        }


        [HttpPost(nameof(issuerMgrVoid))]
        public async Task<IActionResult> issuerMgrVoid([FromForm] IssueSubmissionParameters data)
        {
            var result = await business.IssueSubmission.issuerMgrVoid(data);
            return Ok(result);
        }

        [HttpPost(nameof(issuerMgrReject))]
        public async Task<IActionResult> issuerMgrReject([FromForm] IssueSubmissionParameters data)
        {
            var result = await business.IssueSubmission.issuerMgrReject(data);
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

        [HttpPost(nameof(pdaActionReject))]
        public async Task<IActionResult> pdaActionReject([FromForm] IssueSubmissionParameters data)
        {
            var result = await business.IssueSubmission.pdaActionReject(data);
            return Ok(result);
        }

        [HttpPost(nameof(ReceiverAction))]
        public async Task<IActionResult> ReceiverAction([FromForm] IssueSubmissionParameters data)
        {
            var result = await business.IssueSubmission.ReceiverAction(data);
            return Ok(result);
        }

        [HttpPost(nameof(ReceiverActionUpdate))]
        public async Task<IActionResult> ReceiverActionUpdate([FromForm] IssueSubmissionParameters data)
        {
            var result = await business.IssueSubmission.ReceiverActionUpdate(data);
            return Ok(result);
        }

        [HttpPost(nameof(ReceiverActionAppeal))]
        public async Task<IActionResult> ReceiverActionAppeal([FromForm] IssueSubmissionParameters data)
        {
            var result = await business.IssueSubmission.ReceiverActionAppeal(data);
            return Ok(result);
        }

        [HttpPost(nameof(ReceiverIssueReject))]
        public async Task<IActionResult> ReceiverIssueReject([FromForm] IssueSubmissionParameters data)
        {
            var result = await business.IssueSubmission.ReceiverIssueReject(data);
            return Ok(result);
        }

        [HttpPost(nameof(ReceiverApproval))]
        public async Task<IActionResult> ReceiverApproval([FromForm] IssueSubmissionParameters data)
        {
            var result = await business.IssueSubmission.ReceiverApproval(data);
            return Ok(result);
        }

        [HttpPost(nameof(ReceiverMngReject))]
        public async Task<IActionResult> ReceiverMngReject([FromForm] IssueSubmissionParameters data)
        {
            var result = await business.IssueSubmission.ReceiverMngReject(data);
            return Ok(result);
        }

        [HttpPost(nameof(ReceiverApprovalToReject))]
        public async Task<IActionResult> ReceiverApprovalToReject([FromForm] IssueSubmissionParameters data)
        {
            var result = await business.IssueSubmission.ReceiverApprovalToReject(data);
            return Ok(result);
        }

        [HttpPost(nameof(PDAReviewerVoid))]
        public async Task<IActionResult> PDAReviewerVoid([FromForm] IssueSubmissionParameters data)
        {
            var result = await business.IssueSubmission.PDAReviewerVoid(data);
            return Ok(result);
        }

        [HttpPost(nameof(PDAReviewerReject))]
        public async Task<IActionResult> PDAReviewerReject([FromForm] IssueSubmissionParameters data)
        {
            var result = await business.IssueSubmission.PDAReviewerReject(data);
            return Ok(result);
        }

        [HttpPost(nameof(PDAReviewerAprove))]
        public async Task<IActionResult> PDAReviewerAprove([FromForm] IssueSubmissionParameters data)
        {
            var result = await business.IssueSubmission.PDAReviewerAprove(data);
            return Ok(result);
        }

        [HttpPost(nameof(ReviewerSubmit))]
        public async Task<IActionResult> ReviewerSubmit([FromForm] IssueSubmissionParameters data)
        {
            var result = await business.IssueSubmission.ReviewerSubmit(data);
            return Ok(result);
        }

        [HttpPost(nameof(ReviewerReject))]
        public async Task<IActionResult> ReviewerReject([FromForm] IssueSubmissionParameters data)
        {
            var result = await business.IssueSubmission.ReviewerReject(data);
            return Ok(result);
        }

        [HttpPost(nameof(pdaActionVoid))]
        public async Task<IActionResult> pdaActionVoid([FromForm] IssueSubmissionParameters data)
        {
            var result = await business.IssueSubmission.pdaActionVoid(data);
            return Ok(result);
        }
        [HttpPost(nameof(cekAvailableCompletePastIssue))]
        public async Task<IActionResult> cekAvailableCompletePastIssue([FromForm] cekAvailableCompletePastIssueParam param)
        {
            var result = await business.IssueSubmission.cekAvailableCompletePastIssue(param);
            return Ok(result);
        }
        [HttpGet(nameof(GetPptTemplate))]
        public async Task<IActionResult> GetPptTemplate()
        {
            var fileBytes = await business.IssueSubmission.GetPptTemplate();
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Template");
        }
    }
}
