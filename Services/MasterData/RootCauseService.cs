using Contracts.Infrastructure;
using Contracts.Repository;
using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Services.Contracts.MasterData;
using Entities.CAR;
using Entities.MasterData;
using Entities.ParamRequest;
using Microsoft.AspNetCore.Http;
using System.Collections;

namespace Services.MasterData
{
    internal sealed class RootCauseService(IDataManager data, ICacheManager memCache, ILocalizationService localization): IRootCauseService
    {
        public async Task<ApiResponse<IEnumerable<string>>> getRootCauseList(int plant)
        {
            var result = await data.RootCause.getRootCauseList(plant);
            return ApiResponse<IEnumerable<string>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<RootCauseCategoryDto>>> GetRootCauseCategory(string search,string SearchADV)
        {
            var result = await data.RootCause.GetRootCauseCategory(search, SearchADV);
            return ApiResponse<IEnumerable<RootCauseCategoryDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<RootCauseCategoryDto>>> InsertNewRootCauseCategory(string RootCauseName, string userId, int plant)
        {
            var result = await data.RootCause.InsertNewRootCauseCategory(RootCauseName, userId, plant);
            return ApiResponse<IEnumerable<RootCauseCategoryDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<RootCauseCategoryDto>>> UpdateNewRootCauseCategory(string RootCauseName, int id, string userId)
        {
            var result = await data.RootCause.UpdateNewRootCauseCategory(RootCauseName,id, userId);
            return ApiResponse<IEnumerable<RootCauseCategoryDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<RootCauseCategoryDto>>> DataDelete(int id, string userId)
        {
            var result = await data.RootCause.DataDelete(id,userId);
            return ApiResponse<IEnumerable<RootCauseCategoryDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<RootCauseCategoryDto>>> DataPermDelete(int id)
        {
            var result = await data.RootCause.DataPermDelete(id);
            return ApiResponse<IEnumerable<RootCauseCategoryDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<RootCauseCategoryDto>>> DataRecover(int id, string userId)
        {
            var result = await data.RootCause.DataRecover(id, userId);
            return ApiResponse<IEnumerable<RootCauseCategoryDto>>.SuccessResponse(result);
        }
        public async Task<byte[]> Template()
        {
            // Call the repository to get the template
            var result = await data.RootCause.Template();

            // You could add business logic here if necessary
            return result;
        }
        public async Task<ApiResponse<IEnumerable<string>>> Import(IFormFile file,  string userId)
        {
            string filePath = Path.Combine( file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var result = await data.RootCause.Import(filePath, userId);

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
