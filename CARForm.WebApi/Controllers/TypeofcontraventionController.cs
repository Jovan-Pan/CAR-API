using Entities.MasterData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TypeofcontraventionController(IServiceManager business) : Controller
    {
        [HttpGet(nameof(GetTypeofcontravention))]
        public async Task<IActionResult> GetTypeofcontravention(string? search, string? SearchADV, bool delflag)
        {
            var result = await business.Typeofcontravention.GetTypeofcontravention(search, SearchADV, delflag);
            return Ok(result);

        }

        [HttpPost(nameof(InsertNewData))]
        public async Task<IActionResult> InsertNewData(CRUDTypeofcontraventionDto CRUDTypeofcontraventionDto)
        {
            var result = await business.Typeofcontravention.InsertNewData(CRUDTypeofcontraventionDto);
            return Ok(result);

        }

        [HttpPost(nameof(UpdateData))]
        public async Task<IActionResult> UpdateData(CRUDTypeofcontraventionDto CRUDTypeofcontraventionDto)
        {
            var result = await business.Typeofcontravention.UpdateData(CRUDTypeofcontraventionDto);
            return Ok(result);

        }

        [HttpPost(nameof(DataDelete))]
        public async Task<IActionResult> DataDelete(CRUDTypeofcontraventionDto CRUDTypeofcontraventionDto)
        {
            var result = await business.Typeofcontravention.DataDelete(CRUDTypeofcontraventionDto);
            return Ok(result);

        }

        [HttpPost(nameof(DataPermDelete))]
        public async Task<IActionResult> DataPermDelete(CRUDTypeofcontraventionDto CRUDTypeofcontraventionDto)
        {
            var result = await business.Typeofcontravention.DataPermDelete(CRUDTypeofcontraventionDto);
            return Ok(result);

        }
        [HttpPost(nameof(DataRecover))]
        public async Task<IActionResult> DataRecover(CRUDTypeofcontraventionDto CRUDTypeofcontraventionDto)
        {
            var result = await business.Typeofcontravention.DataRecover(CRUDTypeofcontraventionDto);
            return Ok(result);

        }

        [HttpGet(nameof(Template))]
        public async Task<IActionResult> Template()
        {
            var fileBytes = await business.Typeofcontravention.Template();
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Template");
        }

        [HttpPost(nameof(Import))]
        public async Task<IActionResult> Import([FromForm] IFormFile file, string userId)
        {

            var result = await business.Typeofcontravention.Import(file, userId);
            return Ok(result);

        }
    }
}
