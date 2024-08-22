using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class IssueSubmissionController(IServiceManager business) : Controller
    {
        [HttpGet(nameof(GetIssueStatus))]
        public async Task<IActionResult> GetIssueStatus(string FormNo)
        {
            try
            {
                var result = await business.IssueSubmission.GetIssueStatus(FormNo);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
