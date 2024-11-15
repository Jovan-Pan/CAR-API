using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ImmidateActionController(IServiceManager business) : Controller
    {
        [HttpGet(nameof(getImmidateActionList))]
        public async Task<IActionResult> getImmidateActionList(int plant)
        {
            var result = await business.ImmAct.getImmidateActionList(plant);
            return Ok(result);

        }

        [HttpGet(nameof(GetImmidateAction))]
        public async Task<IActionResult> GetImmidateAction(string? search,string? SearchADV,bool delflag)
        {
            var result = await business.ImmAct.GetImmidateAction(search, SearchADV, delflag);
            return Ok(result);
        }
        [HttpPost(nameof(InsertNewImmidateAction))]
        public async Task<IActionResult> InsertNewImmidateAction([FromForm] string ImmidateName, [FromForm] int plant, [FromForm] string userId)
        {
            var result = await business.ImmAct.InsertNewImmidateAction(ImmidateName, plant, userId);
            return Ok(result);

        }
        [HttpPost(nameof(UpdateImmidateAction))]
        public async Task<IActionResult> UpdateImmidateAction([FromForm] string ImmidateName, [FromForm] int id, [FromForm] string userId)
        {
            var result = await business.ImmAct.UpdateImmidateAction(ImmidateName, id, userId);
            return Ok(result);

        }
        [HttpPost(nameof(DataDelete))]
        public async Task<IActionResult> DataDelete( [FromForm] int id, [FromForm] string userId)
        {
            var result = await business.ImmAct.DataDelete(id,userId);
            return Ok(result);

        }
        [HttpPost(nameof(DataPermDelete))]
        public async Task<IActionResult> DataPermDelete([FromForm] int id)
        {
            var result = await business.ImmAct.DataPermDelete(id);
            return Ok(result);

        }
        [HttpPost(nameof(DataRecover))]
        public async Task<IActionResult> DataRecover([FromForm] int id, [FromForm] string userId)
        {
            var result = await business.ImmAct.DataRecover(id, userId);
            return Ok(result);

        }
        [HttpGet(nameof(Template))]
        public async Task<IActionResult> Template()
        {
            var fileBytes = await business.ImmAct.Template();
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Template");
        }
        
        [HttpPost(nameof(Import))]
        public async Task<IActionResult> Import([FromForm] IFormFile file, string userId)
        {

            var result = await business.ImmAct.Import(file, userId);
            return Ok(result);

        }
    }
}
