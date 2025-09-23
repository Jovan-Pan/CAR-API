using Entities.MasterData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessController(IServiceManager business) : Controller
    {
        [HttpGet(nameof(GetProcess))]
        public async Task<IActionResult> GetProcess([FromQuery] GETProcess GETProcess)
        {
            var result = await business.Process.GetProcess(GETProcess);
            return Ok(result);

        }

        [HttpPost(nameof(InsertNewData))]
        public async Task<IActionResult> InsertNewData(CRUDProcessDto CRUDProcessDto)
        {
            var result = await business.Process.InsertNewData(CRUDProcessDto);
            return Ok(result);

        }

        [HttpPost(nameof(UpdateData))]
        public async Task<IActionResult> UpdateData(CRUDProcessDto CRUDProcessDto)
        {
            var result = await business.Process.UpdateData(CRUDProcessDto);
            return Ok(result);

        }

        [HttpPost(nameof(DataDelete))]
        public async Task<IActionResult> DataDelete(CRUDProcessDto CRUDProcessDto)
        {
            var result = await business.Process.DataDelete(CRUDProcessDto);
            return Ok(result);

        }

        [HttpPost(nameof(DataPermDelete))]
        public async Task<IActionResult> DataPermDelete(CRUDProcessDto CRUDProcessDto)
        {
            var result = await business.Process.DataPermDelete(CRUDProcessDto);
            return Ok(result);

        }
        [HttpPost(nameof(DataRecover))]
        public async Task<IActionResult> DataRecover(CRUDProcessDto CRUDProcessDto)
        {
            var result = await business.Process.DataRecover(CRUDProcessDto);
            return Ok(result);

        }

        [HttpGet(nameof(Template))]
        public async Task<IActionResult> Template()
        {
            var fileBytes = await business.Process.Template();
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Template");
        }

        [HttpPost(nameof(Import))]
        public async Task<IActionResult> Import([FromForm] IFormFile file, string userId)
        {

            var result = await business.Process.Import(file, userId);
            return Ok(result);

        }
    }
}
