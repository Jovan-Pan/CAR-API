using Entities.MasterData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CARCategoryController(IServiceManager business) : Controller
    {
        [HttpGet(nameof(GetCARCategory))]
        public async Task<IActionResult> GetCARCategory(string? search, string? SearchADV, bool delflag)
        {
            var result = await business.CARCategory.GetCARCategory(search, SearchADV, delflag);
            return Ok(result);
        }

        [HttpPost(nameof(InsertNewCARCategory))]
        public async Task<IActionResult> InsertNewCARCategory(CRUDCARCategoryDto CRUDCARCategoryDto)
        {
            var result = await business.CARCategory.InsertCARCategory(CRUDCARCategoryDto);
            return Ok(result);
        }

        [HttpPost(nameof(UpdateNewCARCategory))]
        public async Task<IActionResult> UpdateNewCARCategory(CRUDCARCategoryDto CRUDCARCategoryDto)
        {
            var result = await business.CARCategory.UpdateCARCategory(CRUDCARCategoryDto);
            return Ok(result);
        }

        [HttpPost(nameof(DataDelete))]
        public async Task<IActionResult> DataDelete(CRUDCARCategoryDto CRUDCARCategoryDto)
        {
            var result = await business.CARCategory.DataDelete(CRUDCARCategoryDto);
            return Ok(result);
        }

        [HttpPost(nameof(DataPermDelete))]
        public async Task<IActionResult> DataPermDelete(CRUDCARCategoryDto CRUDCARCategoryDto)
        {
            var result = await business.CARCategory.DataPermDelete(CRUDCARCategoryDto);
            return Ok(result);
        }

        [HttpPost(nameof(DataRecover))]
        public async Task<IActionResult> DataRecover(CRUDCARCategoryDto CRUDCARCategoryDto)
        {
            var result = await business.CARCategory.DataRecover(CRUDCARCategoryDto);
            return Ok(result);
        }

        [HttpGet(nameof(Template))]
        public async Task<IActionResult> Template()
        {
            var fileBytes = await business.CARCategory.Template();
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "Template");
        }

        [HttpPost(nameof(Import))]
        public async Task<IActionResult> Import([FromForm] IFormFile file, string userId)
        {
            var result = await business.CARCategory.Import(file, userId);
            return Ok(result);
        }
    }
}
