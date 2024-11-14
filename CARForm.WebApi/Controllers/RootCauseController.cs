using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System.Web;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RootCauseController(IServiceManager business) : Controller
    {
        [HttpGet(nameof(getRootCauseList))]
        public async Task<IActionResult> getRootCauseList(int plant)
        {
            var result = await business.RootCause.getRootCauseList(plant);
            return Ok(result);

        }
        
        [HttpGet(nameof(GetRootCauseCategory))]
        public async Task<IActionResult> GetRootCauseCategory(string? search, string? SearchADV, bool delflag)
        {
            var result = await business.RootCause.GetRootCauseCategory(search,SearchADV, delflag);
            return Ok(result);

        }

        [HttpPost(nameof(InsertNewRootCauseCategory))]
        public async Task<IActionResult> InsertNewRootCauseCategory([FromForm] string RootCauseName, [FromForm] string userId, [FromForm] int plant)
        {
            var result = await business.RootCause.InsertNewRootCauseCategory(RootCauseName, userId, plant);
            return Ok(result);

        }
        
        [HttpPost(nameof(UpdateNewRootCauseCategory))]
        public async Task<IActionResult> UpdateNewRootCauseCategory([FromForm] string RootCauseName, [FromForm] int id, [FromForm] string userId)
        {
            var result = await business.RootCause.UpdateNewRootCauseCategory(RootCauseName,id, userId);
            return Ok(result);

        }
        
        [HttpPost(nameof(DataDelete))]
        public async Task<IActionResult> DataDelete([FromForm] int id, [FromForm] string userId)
        {
            var result = await business.RootCause.DataDelete(id,userId);
            return Ok(result);

        }
        
        [HttpPost(nameof(DataPermDelete))]
        public async Task<IActionResult> DataPermDelete([FromForm] int id)
        {
            var result = await business.RootCause.DataPermDelete(id);
            return Ok(result);

        }
        [HttpPost(nameof(DataRecover))]
        public async Task<IActionResult> DataRecover([FromForm] int id, [FromForm] string userId)
        {
            var result = await business.RootCause.DataRecover(id,userId);
            return Ok(result);

        }
        
        [HttpGet(nameof(Template))]
        public async Task<IActionResult> Template()
        {
            var fileBytes = await business.RootCause.Template();
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Template");
        }
        
        [HttpPost(nameof(Import))]
        public async Task<IActionResult> Import([FromForm]  IFormFile file, string userId)
        {

            var result = await business.RootCause.Import(file, userId);
            return Ok(result);

        }
    }
}
