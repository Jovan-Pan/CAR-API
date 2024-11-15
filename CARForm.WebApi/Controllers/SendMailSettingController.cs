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
        
        [HttpGet(nameof(GetSendMailSetting))]
        public async Task<IActionResult> GetSendMailSetting(string? search, string? ATsearchADV,  string? ATDsearchADV, bool delflag)
        {
            var result = await business.sendmailsetting.GetSendMailSetting(search, ATsearchADV, ATDsearchADV, delflag);
            return Ok(result);
        }
        
        [HttpPost(nameof(InsertNewSendMailSetting))]
        public async Task<IActionResult> InsertNewSendMailSetting([FromForm] string plant, [FromForm] string actiontype, [FromForm] string actiontypedesc, [FromForm] bool issendemail, [FromForm] string userId)
        {
            var result = await business.sendmailsetting.InsertNewSendMailSetting(plant, actiontype, actiontypedesc, issendemail,userId);
            return Ok(result);
        }
        
        [HttpPost(nameof(UpdateSendMailSetting))]
        public async Task<IActionResult> UpdateSendMailSetting([FromForm] string actiontype, [FromForm] string actiontypedesc, [FromForm] bool issendemail, [FromForm] string userId)
        {
            var result = await business.sendmailsetting.UpdateSendMailSetting(actiontype, actiontypedesc, issendemail, userId);
            return Ok(result);
        }
        
        [HttpPost(nameof(DataDelete))]
        public async Task<IActionResult> DataDelete([FromForm] string actiontype, [FromForm] string userId)
        {
            var result = await business.sendmailsetting.DataDelete(actiontype,userId);
            return Ok(result);
        }
        
        [HttpPost(nameof(DataPermDelete))]
        public async Task<IActionResult> DataPermDelete([FromForm] string actiontype)
        {
            var result = await business.sendmailsetting.DataPermDelete(actiontype);
            return Ok(result);
        }
        
        [HttpPost(nameof(DataRecover))]
        public async Task<IActionResult> DataRecover([FromForm] string actiontype, [FromForm] string userId)
        {
            var result = await business.sendmailsetting.DataRecover(actiontype,userId);
            return Ok(result);
        }
        
        [HttpGet(nameof(Template))]
        public async Task<IActionResult> Template()
        {
            var fileBytes = await business.sendmailsetting.Template();
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Template");
        }
        
        [HttpPost(nameof(Import))]
        public async Task<IActionResult> Import([FromForm] IFormFile file, string userId)
        {

            var result = await business.sendmailsetting.Import(file, userId);
            return Ok(result);

        }
    }
}
