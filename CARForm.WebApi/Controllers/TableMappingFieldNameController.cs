using Entities.MasterData;
using Entities.ParamRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TableMappingFieldNameController(IServiceManager business) : Controller
    {
        [HttpGet(nameof(GetTableMappingFieldName))]
        public async Task<IActionResult> GetTableMappingFieldName(string? search, string? SearchADVFN, string? SearchADVUID, string? SearchADVLANG, bool delflag,string? plant)
        {
            var result = await business.TableMappingFieldName.GetTableMappingFieldName(search, SearchADVFN, SearchADVUID, SearchADVLANG, delflag, plant);
            return Ok(result);

        }

        [HttpPost(nameof(CheckExistingData))]
        public async Task<IActionResult> CheckExistingData(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto)
        {
            var result = await business.TableMappingFieldName.CheckExistingData(CRUDTableMappingFieldNameDto);
            return Ok(result);
        }

        [HttpPost(nameof(InsertNewData))]
        public async Task<IActionResult> InsertNewData(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto)
        {
            var result = await business.TableMappingFieldName.InsertNewData(CRUDTableMappingFieldNameDto);
            return Ok(result);

        }

        [HttpPost(nameof(UpdateData))]
        public async Task<IActionResult> UpdateData(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto)
        {
            var result = await business.TableMappingFieldName.UpdateData(CRUDTableMappingFieldNameDto);
            return Ok(result);

        }

        [HttpPost(nameof(DataDelete))]
        public async Task<IActionResult> DataDelete(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto)
        {
            var result = await business.TableMappingFieldName.DataDelete(CRUDTableMappingFieldNameDto);
            return Ok(result);

        }

        [HttpPost(nameof(DataPermDelete))]
        public async Task<IActionResult> DataPermDelete(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto)
        {
            var result = await business.TableMappingFieldName.DataPermDelete(CRUDTableMappingFieldNameDto);
            return Ok(result);

        }
        [HttpPost(nameof(DataRecover))]
        public async Task<IActionResult> DataRecover(CRUDTableMappingFieldNameDto CRUDTableMappingFieldNameDto)
        {
            var result = await business.TableMappingFieldName.DataRecover(CRUDTableMappingFieldNameDto);
            return Ok(result);

        }

        [HttpGet(nameof(Template))]
        public async Task<IActionResult> Template()
        {
            var fileBytes = await business.TableMappingFieldName.Template();
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Template");
        }

        [HttpPost(nameof(Import))]
        public async Task<IActionResult> Import([FromForm] IFormFile file, string userId)
        {

            var result = await business.TableMappingFieldName.Import(file, userId);
            return Ok(result);

        }
    }
}
