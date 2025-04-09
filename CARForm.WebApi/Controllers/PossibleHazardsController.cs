using Entities.MasterData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PossibleHazardsController(IServiceManager business) : Controller
    {
        [HttpGet(nameof(GetPossibleHazards))]
        public async Task<IActionResult> GetPossibleHazards([FromQuery] GETPossibleHazards GETPossibleHazards)
        {
            var result = await business.PossibleHazards.GetPossibleHazards(GETPossibleHazards);
            return Ok(result);

        }

        [HttpPost(nameof(InsertNewData))]
        public async Task<IActionResult> InsertNewData(CRUDPossibleHazardsDto CRUDPossibleHazardsDto)
        {
            var result = await business.PossibleHazards.InsertNewData(CRUDPossibleHazardsDto);
            return Ok(result);

        }

        [HttpPost(nameof(UpdateData))]
        public async Task<IActionResult> UpdateData(CRUDPossibleHazardsDto CRUDPossibleHazardsDto)
        {
            var result = await business.PossibleHazards.UpdateData(CRUDPossibleHazardsDto);
            return Ok(result);

        }

        [HttpPost(nameof(DataDelete))]
        public async Task<IActionResult> DataDelete(CRUDPossibleHazardsDto CRUDPossibleHazardsDto)
        {
            var result = await business.PossibleHazards.DataDelete(CRUDPossibleHazardsDto);
            return Ok(result);

        }

        [HttpPost(nameof(DataPermDelete))]
        public async Task<IActionResult> DataPermDelete(CRUDPossibleHazardsDto CRUDPossibleHazardsDto)
        {
            var result = await business.PossibleHazards.DataPermDelete(CRUDPossibleHazardsDto);
            return Ok(result);

        }
        [HttpPost(nameof(DataRecover))]
        public async Task<IActionResult> DataRecover(CRUDPossibleHazardsDto CRUDPossibleHazardsDto)
        {
            var result = await business.PossibleHazards.DataRecover(CRUDPossibleHazardsDto);
            return Ok(result);

        }

        [HttpGet(nameof(Template))]
        public async Task<IActionResult> Template()
        {
            var fileBytes = await business.PossibleHazards.Template();
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Template");
        }

        [HttpPost(nameof(Import))]
        public async Task<IActionResult> Import([FromForm] IFormFile file, string userId)
        {

            var result = await business.PossibleHazards.Import(file, userId);
            return Ok(result);

        }
    }
}
