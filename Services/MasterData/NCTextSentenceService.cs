using Entities.MasterData;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Contracts.Infrastructure;
using Contracts.Repository;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts.MasterData;
using Microsoft.AspNetCore.Http;

namespace Services.MasterData
{
    internal sealed class NCTextSentenceService(IDataManager data, ICacheManager memCache, ILocalizationService localization):INCTextSentenceService
    {
        public async Task<ApiResponse<IEnumerable<NCTextSentenceDto>>> GetDataNcTextSentence(string search, string SearchADV)
        {
            var result = await data.NCTS.GetDataNcTextSentence(search, SearchADV);
            return ApiResponse<IEnumerable<NCTextSentenceDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<NCTextSentenceDto>>> InsertDataNcTextSentence(string TextSentence, bool isFirstSentence, bool isLastSentence, string userId)
        {
            var result = await data.NCTS.InsertDataNcTextSentence(TextSentence, isFirstSentence, isLastSentence, userId);
            return ApiResponse<IEnumerable<NCTextSentenceDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<NCTextSentenceDto>>> UpdateDataNcTextSentence(int id, string TextSentence, bool isFirstSentence, bool isLastSentence, string userId)
        {
            var result = await data.NCTS.UpdateDataNcTextSentence(id,TextSentence, isFirstSentence, isLastSentence, userId);
            return ApiResponse<IEnumerable<NCTextSentenceDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<NCTextSentenceDto>>> DataDelete(int id, string userId)
        {
            var result = await data.NCTS.DataDelete(id, userId);
            return ApiResponse<IEnumerable<NCTextSentenceDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<NCTextSentenceDto>>> DataPermDelete(string TextSentence)
        {
            var result = await data.NCTS.DataPermDelete(TextSentence);
            return ApiResponse<IEnumerable<NCTextSentenceDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<NCTextSentenceDto>>> DataRecover(int id, string userId)
        {
            var result = await data.NCTS.DataRecover(id, userId);
            return ApiResponse<IEnumerable<NCTextSentenceDto>>.SuccessResponse(result);
        }
        public async Task<byte[]> Template()
        {
            var result = await data.NCTS.Template();
            return result;
        }
        public async Task<ApiResponse<IEnumerable<string>>> Import(IFormFile file, string userId)
        {
            string filePath = Path.Combine(file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var result = await data.NCTS.Import(filePath, userId);

            System.IO.File.Delete(filePath);

            if (result.Contains("Invalid Data structure, please follow template format"))
            {
                return new ApiResponse<IEnumerable<string>>
                {
                    Success = false,
                    Message = "Invalid Data structure, please follow template format",
                    Content = null
                };
            }

            return ApiResponse<IEnumerable<string>>.SuccessResponse(result);
        }
    }
}
