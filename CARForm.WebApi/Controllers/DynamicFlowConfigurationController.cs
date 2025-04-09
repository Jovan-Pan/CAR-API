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
    public class DynamicFlowConfigurationController(IServiceManager business) : Controller
    {
        [HttpGet(nameof(GetDynamicFlowConfiguration))]
        public async Task<IActionResult> GetDynamicFlowConfiguration(string? search, string? SearchFTADV, string? SearchADV, bool delflag)
        {
            var result = await business.DynamicFlowConfiguration.GetDynamicFlowConfiguration(search, SearchFTADV, SearchADV, delflag);
            return Ok(result);

        }

        [HttpPost(nameof(InsertNewData))]
        public async Task<IActionResult> InsertNewData(CRUDDynamicFlowConfigurationDto CRUDDynamicFlowConfigurationDto)
        {
            var result = await business.DynamicFlowConfiguration.InsertNewData(CRUDDynamicFlowConfigurationDto);
            return Ok(result);

        }

        [HttpPost(nameof(UpdateData))]
        public async Task<IActionResult> UpdateData(CRUDDynamicFlowConfigurationDto CRUDDynamicFlowConfigurationDto)
        {
            var result = await business.DynamicFlowConfiguration.UpdateData(CRUDDynamicFlowConfigurationDto);
            return Ok(result);

        }

        [HttpPost(nameof(DataDelete))]
        public async Task<IActionResult> DataDelete(CRUDDynamicFlowConfigurationDto CRUDDynamicFlowConfigurationDto)
        {
            var result = await business.DynamicFlowConfiguration.DataDelete(CRUDDynamicFlowConfigurationDto);
            return Ok(result);

        }

        [HttpPost(nameof(DataPermDelete))]
        public async Task<IActionResult> DataPermDelete(CRUDDynamicFlowConfigurationDto CRUDDynamicFlowConfigurationDto)
        {
            var result = await business.DynamicFlowConfiguration.DataPermDelete(CRUDDynamicFlowConfigurationDto);
            return Ok(result);

        }
        [HttpPost(nameof(DataRecover))]
        public async Task<IActionResult> DataRecover(CRUDDynamicFlowConfigurationDto CRUDDynamicFlowConfigurationDto)
        {
            var result = await business.DynamicFlowConfiguration.DataRecover(CRUDDynamicFlowConfigurationDto);
            return Ok(result);

        }

        [HttpGet(nameof(Template))]
        public async Task<IActionResult> Template()
        {
            var fileBytes = await business.DynamicFlowConfiguration.Template();
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Template");
        }

        [HttpPost(nameof(Import))]
        public async Task<IActionResult> Import([FromForm] IFormFile file, string userId)
        {

            var result = await business.DynamicFlowConfiguration.Import(file, userId);
            return Ok(result);

        }
    }
}
