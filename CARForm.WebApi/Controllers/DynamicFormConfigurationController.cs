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
    public class DynamicFormConfigurationController(IServiceManager business) : Controller
    {
        [AllowAnonymous]
        [HttpGet(nameof(GetIssueFeedbackcolumn))]
        public async Task<IActionResult> GetIssueFeedbackcolumn()
        {
            var result = await business.DynamicFormConfiguration.GetIssueFeedbackColumn();
            return Ok(result);

        }
        [AllowAnonymous]
        [HttpPost(nameof(GetTestingQueryResult))]
        public async Task<IActionResult> GetTestingQueryResult(TestingQueryParam TestingQueryParam)
        {
            var result = await business.DynamicFormConfiguration.GetTestingQueryResult(TestingQueryParam);
            return Ok(result);

        }
        [AllowAnonymous]
        [HttpPost(nameof(InsertNewData))]
        public async Task<IActionResult> InsertNewData(DynamicFormConfigurationDto DynamicFormConfigurationDto)
            {
            var result = await business.DynamicFormConfiguration.InsertNewData(DynamicFormConfigurationDto);
            return Ok(result);  

        }
        [AllowAnonymous]
        [HttpGet(nameof(GetDynamicFormConfiguration))]
        public async Task<IActionResult> GetDynamicFormConfiguration(string Language,string Plant, string? search, bool delflag, string? formTypeAdv,string? fieldNameAdv,string? fieldTypeAdv,string? fieldLengthAdv,string? mandatoryAdv,string? fieldElementAdv,string? optionDataResourceAdv,string? dbResourceAdv,string? queryAdv,string? dataOptionAdv,string? sequenceAdv)
        {
            var result = await business.DynamicFormConfiguration.GetDynamicFormConfiguration(Language, Plant,search, delflag,formTypeAdv,fieldNameAdv,fieldTypeAdv,fieldLengthAdv,mandatoryAdv,fieldElementAdv,optionDataResourceAdv,dbResourceAdv,queryAdv,dataOptionAdv,sequenceAdv);
           return Ok(result);

        }
        [HttpPost(nameof(UpdateData))]
        public async Task<IActionResult> UpdateData(DynamicFormConfigurationDto DynamicFormConfigurationDto)
        {
            var result = await business.DynamicFormConfiguration.UpdateData(DynamicFormConfigurationDto);
            return Ok(result);

        }

        [HttpPost(nameof(DataDelete))]
        public async Task<IActionResult> DataDelete(DynamicFormConfigurationDto DynamicFormConfigurationDto)
        {
            var result = await business.DynamicFormConfiguration.DataDelete(DynamicFormConfigurationDto);
            return Ok(result);

        }

        [HttpPost(nameof(DataPermDelete))]
        public async Task<IActionResult> DataPermDelete(DynamicFormConfigurationDto DynamicFormConfigurationDto)
        {
            var result = await business.DynamicFormConfiguration.DataPermDelete(DynamicFormConfigurationDto);
            return Ok(result);

        }
        [HttpPost(nameof(DataRecover))]
        public async Task<IActionResult> DataRecover(DynamicFormConfigurationDto DynamicFormConfigurationDto)
        {
            var result = await business.DynamicFormConfiguration.DataRecover(DynamicFormConfigurationDto);
            return Ok(result);

        }

        [HttpGet(nameof(Template))]
        public async Task<IActionResult> Template()
        {
            var fileBytes = await business.DynamicFormConfiguration.Template();
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Template");
        }

        [HttpPost(nameof(Import))]
        public async Task<IActionResult> Import([FromForm] IFormFile file, string userId)
        {

            var result = await business.DynamicFormConfiguration.Import(file, userId);
            return Ok(result);

        }
    }
}
