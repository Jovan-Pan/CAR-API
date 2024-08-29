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
        [HttpPost(nameof(GetDataReport))]
        public async Task<IActionResult> GetDataReport([FromBody] GlobalParam data)
        {
            var result = await business.IFR.GetDataReport(data);

            //var issueFeedbackResult = result.Content;

            //// Serialize files to base64 strings or URLs
            //var serializedFiles = new
            //{
            //    NCCategoryImgFiles = issueFeedbackResult.formFiles?.NCCategoryImgFiles.Select(file => new
            //    {
            //        file.FileName,
            //        file.ContentType,
            //        Content = ConvertToBase64(file) // Convert to base64
            //    }),
            //    NCCategoryFiles = issueFeedbackResult.formFiles?.NCCategoryFiles.Select(file => new
            //    {
            //        file.FileName,
            //        file.ContentType,
            //        Content = ConvertToBase64(file) // Convert to base64
            //    })
            //};

            //// Combine the data and serialized files into a single object
            //var response = new
            //{
            //    success = result.Success,
            //    message = result.Message,
            //    issueFeedbackResult.maindata,
            //    issueFeedbackResult.dataAtch,
            //    Files = serializedFiles,
            //    issueFeedbackResult.totrecord
            //};

            return Ok(result);
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
