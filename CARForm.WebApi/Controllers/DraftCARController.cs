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
        public async Task<IActionResult> Import([FromForm] IFormFile file, string userId, string username)
        {
            var result = await business.DraftCAR.Import(file, userId, username);
            return Ok(result);
        }
    }
}
