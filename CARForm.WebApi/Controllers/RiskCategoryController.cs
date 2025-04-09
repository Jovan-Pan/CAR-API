using Entities.MasterData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RiskCategoryController(IServiceManager business) : Controller
    {
        [HttpGet(nameof(GetRiskCategory))]
        public async Task<IActionResult> GetRiskCategory([FromQuery] GETRiskCategory GETRiskCategory)
        {
            var result = await business.RiskCategory.GetRiskCategory(GETRiskCategory);
            return Ok(result);

        }

        [HttpPost(nameof(InsertNewData))]
        public async Task<IActionResult> InsertNewData(CRUDRiskCategoryDto CRUDRiskCategoryDto)
        {
            var result = await business.RiskCategory.InsertNewData(CRUDRiskCategoryDto);
            return Ok(result);

        }

        [HttpPost(nameof(UpdateData))]
        public async Task<IActionResult> UpdateData(CRUDRiskCategoryDto CRUDRiskCategoryDto)
        {
            var result = await business.RiskCategory.UpdateData(CRUDRiskCategoryDto);
            return Ok(result);

        }

        [HttpPost(nameof(DataDelete))]
        public async Task<IActionResult> DataDelete(CRUDRiskCategoryDto CRUDRiskCategoryDto)
        {
            var result = await business.RiskCategory.DataDelete(CRUDRiskCategoryDto);
            return Ok(result);

        }

        [HttpPost(nameof(DataPermDelete))]
        public async Task<IActionResult> DataPermDelete(CRUDRiskCategoryDto CRUDRiskCategoryDto)
        {
            var result = await business.RiskCategory.DataPermDelete(CRUDRiskCategoryDto);
            return Ok(result);

        }
        [HttpPost(nameof(DataRecover))]
        public async Task<IActionResult> DataRecover(CRUDRiskCategoryDto CRUDRiskCategoryDto)
        {
            var result = await business.RiskCategory.DataRecover(CRUDRiskCategoryDto);
            return Ok(result);

        }

        [HttpGet(nameof(Template))]
        public async Task<IActionResult> Template()
        {
            var fileBytes = await business.RiskCategory.Template();
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Template");
        }

        [HttpPost(nameof(Import))]
        public async Task<IActionResult> Import([FromForm] IFormFile file, string userId)
        {

            var result = await business.RiskCategory.Import(file, userId);
            return Ok(result);

        }
    }
}
