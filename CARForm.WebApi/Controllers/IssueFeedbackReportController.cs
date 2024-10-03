using Entities.ParamRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services.Contracts;
using System.Net.Http.Headers;
using System.Text;

namespace WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class IssueFeedbackReportController(IServiceManager business) : Controller
    {
        [HttpPost(nameof(GetTotalRecordForEachStts))]
        public async Task<IActionResult> GetTotalRecordForEachStts([FromForm] GetTotalRecordForEachSttsParam param)
        {
            var result = await business.IFR.GetTotalRecordForEachStts(param);
            return Ok(result);
        }

        [HttpPost(nameof(GetFormNumberListFilter))]
        public async Task<IActionResult> GetFormNumberListFilter([FromForm] int plant, [FromForm] IEnumerable<string> deptAuthList, [FromForm] IEnumerable<string> productAuthList)
        {
            var result = await business.IFR.GetFormNumberListFilter(plant, deptAuthList, productAuthList);
            return Ok(result);
        }

        [HttpPost(nameof(GetDeptListFilter))]
        public async Task<IActionResult> GetDeptListFilter([FromForm] int plant, [FromForm] IEnumerable<string> deptAuthList, [FromForm] IEnumerable<string> productAuthList)
        {
            var result = await business.IFR.GetDeptListFilter(plant, deptAuthList, productAuthList);
            return Ok(result);

        }

        [HttpPost(nameof(GetprocecessGrpCodeFilter))]
        public async Task<IActionResult> GetprocecessGrpCodeFilter([FromForm] int plant, [FromForm] IEnumerable<string> deptAuthList, [FromForm] IEnumerable<string> productAuthList)
        {
            var result = await business.IFR.GetprocecessGrpCodeFilter(plant, deptAuthList, productAuthList);
            return Ok(result);

        }

        [HttpPost(nameof(GetproductListFilter))]
        public async Task<IActionResult> GetproductListFilter([FromForm] int plant, [FromForm] IEnumerable<string> deptAuthList, [FromForm] IEnumerable<string> productAuthList)
        {
            var result = await business.IFR.GetproductListFilter(plant, deptAuthList, productAuthList);
            return Ok(result);

        }

        [HttpPost(nameof(GetModelListFilter))]
        public async Task<IActionResult> GetModelListFilter([FromForm] int plant, [FromForm] IEnumerable<string> deptAuthList, [FromForm] IEnumerable<string> productAuthList)
        {
            var result = await business.IFR.GetModelListFilter(plant, deptAuthList,productAuthList);
            return Ok(result);
        }

        [HttpPost(nameof(GetMatTypeListFilter))]
        public async Task<IActionResult> GetMatTypeListFilter([FromForm] int plant, [FromForm] IEnumerable<string> deptAuthList, [FromForm] IEnumerable<string> productAuthList)
        {
            var result = await business.IFR.GetMatTypeListFilter(plant, deptAuthList, productAuthList);
            return Ok(result);
        }

        [HttpPost(nameof(GetMaterialListFilter))]
        public async Task<IActionResult> GetMaterialListFilter([FromForm] int plant, [FromForm] IEnumerable<string> deptAuthList, [FromForm] IEnumerable<string> productAuthList, [FromForm] string searchTerm)
        {
            var result = await business.IFR.GetMaterialListFilter(plant, deptAuthList, productAuthList, searchTerm);
            return Ok(result);

        }

        [HttpPost(nameof(GetVendorListFilter))]
        public async Task<IActionResult> GetVendorListFilter([FromForm] int plant, [FromForm] IEnumerable<string> deptAuthList, [FromForm] IEnumerable<string> productAuthList)
        {
            var result = await business.IFR.GetVendorListFilter(plant, deptAuthList, productAuthList);
            return Ok(result);
        }

        [HttpPost(nameof(GetDataReport))]
        public async Task<IActionResult> GetDataReport([FromBody] GlobalParam data)
        {
            var result = await business.IFR.GetDataReport(data);


            return Ok(result);
        }

        [HttpPost(nameof(FilePreview))]
        public async Task<IActionResult> FilePreview([FromBody] GetAttachmentParam request)
        {
            try
            {
                var (fileStream, mimeType, fileName) = await business.IFR.GetFilePreviewAsync(request);
                return File(fileStream, mimeType, fileName);
            }
            catch (FileNotFoundException)
            {
                return NotFound("File not found.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost(nameof(GetFilesAttchment))]
        public async Task<IActionResult> GetFilesAttchment([FromBody] IEnumerable<GetAttachmentParam> request)
        {
            try
            {
                var files = await business.IFR.GetFilesAttchment(request);

                if (files == null || files.Count == 0)
                {
                    return NotFound("Files not found.");
                }

                return Ok(files.Select(f => new
                {
                    Base64Content = f.Base64Content,
                    MimeType = f.MimeType,
                    FileName = f.FileName
                }));
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private string ConvertToBase64(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                var fileBytes = memoryStream.ToArray();
                return Convert.ToBase64String(fileBytes);
            }
        }
    }


}
