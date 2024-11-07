using Entities.ParamRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NcTextSentenceController(IServiceManager business) : Controller
    {
        [AllowAnonymous]
        [HttpPost(nameof(GetDataNcTextSentence))]
        public async Task<IActionResult> GetDataNcTextSentence([FromForm] string? search, [FromForm] string? SearchADV)
        {
            var result = await business.NCTS.GetDataNcTextSentence(search, SearchADV);
            return Ok(result);
        }
        [HttpPost(nameof(InsertDataNcTextSentence))]
        public async Task<IActionResult> InsertDataNcTextSentence([FromForm] string TextSentence, [FromForm] bool isFirstSentence, [FromForm] bool isLastSentence, [FromForm] string userId)
        {
            var result = await business.NCTS.InsertDataNcTextSentence(TextSentence, isFirstSentence, isLastSentence, userId);
            return Ok(result);
        }
        [HttpPost(nameof(UpdateDataNcTextSentence))]
        public async Task<IActionResult> UpdateDataNcTextSentence([FromForm]int id, [FromForm] string TextSentence, [FromForm] bool isFirstSentence, [FromForm] bool isLastSentence, [FromForm] string userId)
        {
            var result = await business.NCTS.UpdateDataNcTextSentence(id,TextSentence, isFirstSentence, isLastSentence, userId);
            return Ok(result);
        }
        [HttpPost(nameof(DataDelete))]
        public async Task<IActionResult> DataDelete([FromForm] int id, [FromForm] string userId)
        {
            var result = await business.NCTS.DataDelete(id, userId);
            return Ok(result);
        }
        [HttpPost(nameof(DataPermDelete))]
        public async Task<IActionResult> DataPermDelete([FromForm] int id)
        {
            var result = await business.NCTS.DataPermDelete(id);
            return Ok(result);
        }
        [HttpPost(nameof(DataRecover))]
        public async Task<IActionResult> DataRecover([FromForm] int id, [FromForm] string userId)
        {
            var result = await business.NCTS.DataRecover(id, userId);
            return Ok(result);
        }
        [HttpGet(nameof(Template))]
        public async Task<IActionResult> Template()
        {
            var fileBytes = await business.NCTS.Template();
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Template");
        }
        [HttpPost(nameof(Import))]
        public async Task<IActionResult> Import([FromForm] IFormFile file, string userId)
        {

            var result = await business.NCTS.Import(file, userId);
            return Ok(result);

        }
    }
}
