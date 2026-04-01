using Entities.CAR;
using Entities.ParamRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicNewFormController(IServiceManager business) : Controller
    {
        [HttpGet(nameof(GetDynamicFormConfiguration))]
        public async Task<IActionResult> GetDynamicFormConfiguration(string userid, string Language, string Plant, bool delflag)
        {
            var result = await business.DynamicNewForm.GetDynamicFormConfiguration(userid, Language, Plant, delflag);
            return Ok(result);

        }
        [HttpPost(nameof(ProcessSubmit))]
        public async Task<IActionResult> ProcessSubmit([FromForm] DynamicFormParameterDTO data)
        {
            var result = await business.DynamicNewForm.ProcessSubmit(data);
            return Ok(result);
        }

        [HttpPost(nameof(issuerUpdate))]
        public async Task<IActionResult> issuerUpdate([FromForm] DynamicFormParameterDTO data)
        {
            var result = await business.DynamicNewForm.issuerUpdate(data);
            return Ok(result);
        }

        [HttpPost(nameof(issuerVoid))]
        public async Task<IActionResult> issuerVoid([FromForm] DynamicFormParameterDTO data)
        {
            var result = await business.DynamicNewForm.issuerVoid(data);
            return Ok(result);
        }


        [HttpPost(nameof(issuerMgrVoid))]
        public async Task<IActionResult> issuerMgrVoid([FromForm] DynamicFormParameterDTO data)
        {
            var result = await business.DynamicNewForm.issuerMgrVoid(data);
            return Ok(result);
        }

        [HttpPost(nameof(issuerMgrReject))]
        public async Task<IActionResult> issuerMgrReject([FromForm] DynamicFormParameterDTO data)
        {
            var result = await business.DynamicNewForm.issuerMgrReject(data);
            return Ok(result);
        }

        [HttpPost(nameof(issuerMngUpdate))]
        public async Task<IActionResult> issuerMngUpdate([FromForm] DynamicFormParameterDTO data)
        {
            var result = await business.DynamicNewForm.issuerMngUpdate(data);
            return Ok(result);
        }

        [HttpPost(nameof(pdaDecision))]
        public async Task<IActionResult> pdaDecision([FromForm] DynamicFormParameterDTO data)
        {
            var result = await business.DynamicNewForm.pdaDecision(data);
            return Ok(result);
        }

        [HttpPost(nameof(pdaDecisionUpdate))]
        public async Task<IActionResult> pdaDecisionUpdate([FromForm] DynamicFormParameterDTO data)
        {
            var result = await business.DynamicNewForm.pdaDecisionUpdate(data);
            return Ok(result);
        }

        [HttpPost(nameof(pdaApproval))]
        public async Task<IActionResult> pdaApproval([FromForm] DynamicFormParameterDTO data)
        {
            var result = await business.DynamicNewForm.pdaApproval(data);
            return Ok(result);
        }

        [HttpPost(nameof(pdaActionReject))]
        public async Task<IActionResult> pdaActionReject([FromForm] DynamicFormParameterDTO data)
        {
            var result = await business.DynamicNewForm.pdaActionReject(data);
            return Ok(result);
        }

        [HttpPost(nameof(ReceiverAction))]
        public async Task<IActionResult> ReceiverAction([FromForm] DynamicFormParameterDTO data)
        {
            var result = await business.DynamicNewForm.ReceiverAction(data);
            return Ok(result);
        }

        [HttpPost(nameof(ReceiverActionUpdate))]
        public async Task<IActionResult> ReceiverActionUpdate([FromForm] DynamicFormParameterDTO data)
        {
            var result = await business.DynamicNewForm.ReceiverActionUpdate(data);
            return Ok(result);
        }

        [HttpPost(nameof(ReceiverActionAppeal))]
        public async Task<IActionResult> ReceiverActionAppeal([FromForm] DynamicFormParameterDTO data)
        {
            var result = await business.DynamicNewForm.ReceiverActionAppeal(data);
            return Ok(result);
        }

        [HttpPost(nameof(ReceiverIssueReject))]
        public async Task<IActionResult> ReceiverIssueReject([FromForm] DynamicFormParameterDTO data)
        {
            var result = await business.DynamicNewForm.ReceiverIssueReject(data);
            return Ok(result);
        }

        [HttpPost(nameof(ReceiverApproval))]
        public async Task<IActionResult> ReceiverApproval([FromForm] DynamicFormParameterDTO data)
        {
            var result = await business.DynamicNewForm.ReceiverApproval(data);
            return Ok(result);
        }

        [HttpPost(nameof(ReceiverMngReject))]
        public async Task<IActionResult> ReceiverMngReject([FromForm] DynamicFormParameterDTO data)
        {
            var result = await business.DynamicNewForm.ReceiverMngReject(data);
            return Ok(result);
        }

        [HttpPost(nameof(ReceiverApprovalToReject))]
        public async Task<IActionResult> ReceiverApprovalToReject([FromForm] DynamicFormParameterDTO data)
        {
            var result = await business.DynamicNewForm.ReceiverApprovalToReject(data);
            return Ok(result);
        }

        [HttpPost(nameof(PDAReviewerVoid))]
        public async Task<IActionResult> PDAReviewerVoid([FromForm] DynamicFormParameterDTO data)
        {
            var result = await business.DynamicNewForm.PDAReviewerVoid(data);
            return Ok(result);
        }

        [HttpPost(nameof(PDAReviewerReject))]
        public async Task<IActionResult> PDAReviewerReject([FromForm] DynamicFormParameterDTO data)
        {
            var result = await business.DynamicNewForm.PDAReviewerReject(data);
            return Ok(result);
        }

        [HttpPost(nameof(PDAReviewerAprove))]
        public async Task<IActionResult> PDAReviewerAprove([FromForm] DynamicFormParameterDTO data)
        {
            var result = await business.DynamicNewForm.PDAReviewerAprove(data);
            return Ok(result);
        }

        [HttpPost(nameof(ReviewerSubmit))]
        public async Task<IActionResult> ReviewerSubmit([FromForm] DynamicFormParameterDTO data)
        {
            var result = await business.DynamicNewForm.ReviewerSubmit(data);
            return Ok(result);
        }

        [HttpPost(nameof(ReviewerReject))]
        public async Task<IActionResult> ReviewerReject([FromForm] DynamicFormParameterDTO data)
        {
            var result = await business.DynamicNewForm.ReviewerReject(data);
            return Ok(result);
        }

        [HttpPost(nameof(cekAvailableCompletePastIssue))]
        public async Task<IActionResult> cekAvailableCompletePastIssue([FromForm] cekAvailableCompletePastIssueParam param)
        {
            var result = await business.DynamicNewForm.cekAvailableCompletePastIssue(param);
            return Ok(result);
        }

        [HttpPost(nameof(AIFiveWhyRootCause))]
        public async Task<IActionResult> AIFiveWhyRootCause([FromForm] DynamicFormParameterDTO data)
        {
            var result = await business.DynamicNewForm.AIFiveWhyRootCause(data);
            return Ok(result);
        }
    }
}
