using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DraftCARController(IServiceManager business) : Controller
    {
        [HttpGet(nameof(Template))]
        public async Task<IActionResult> Template()
        {
            var fileBytes = await business.DraftCAR.Template();
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Template");
        }

        [HttpPost(nameof(Import))]
        public async Task<IActionResult> Import([FromForm] IFormFile file, [FromForm] string userId, [FromForm] string username
            , [FromForm] int plant
            , [FromForm] IEnumerable<string> deptAuthList, [FromForm] IEnumerable<string> productAuthList
            )
        {
            var result = await business.DraftCAR.Import(file, userId, username, plant, deptAuthList, productAuthList);
            return Ok(result);
        }
    }
}
