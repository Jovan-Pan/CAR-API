using Entities.MasterData;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts.Infrastructure;
using Contracts.Repository.MasterData;
using Contracts;
using Services.Resources;
using Services.Contracts.MasterData;
using Contracts.Repository;
using Entities.CAR;
using Microsoft.AspNetCore.Http;

namespace Services.MasterData
{
    internal sealed class ImmidateActionService(IDataManager data, ICacheManager memCache, ILocalizationService localization): IImmidateActionService
    {
        public async Task<ApiResponse<IEnumerable<string>>> getImmidateActionList(int plant)
        {
            var result = await data.ImmAct.getImmidateActionList(plant);
            return ApiResponse<IEnumerable<string>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<ImmidateActionDto>>> GetImmidateAction(string? search, string? SearchADV, bool delflag)
        {
            var result = await data.ImmAct.GetImmidateAction(search, SearchADV,delflag);
            return ApiResponse<IEnumerable<ImmidateActionDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<ImmidateActionDto>>> InsertNewImmidateAction(string ImmidateName, int plant, string userId)
        {
            var result = await data.ImmAct.InsertNewImmidateAction(ImmidateName, plant, userId);
            return ApiResponse<IEnumerable<ImmidateActionDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<ImmidateActionDto>>> UpdateImmidateAction(string ImmidateName, int id, string userId)
        {
            var result = await data.ImmAct.UpdateImmidateAction(ImmidateName, id,userId);
            return ApiResponse<IEnumerable<ImmidateActionDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<ImmidateActionDto>>> DataDelete( int id, string userId)
        {
            var result = await data.ImmAct.DataDelete( id, userId);
            return ApiResponse<IEnumerable<ImmidateActionDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<ImmidateActionDto>>> DataPermDelete(int id)
        {
            var result = await data.ImmAct.DataPermDelete(id);
            return ApiResponse<IEnumerable<ImmidateActionDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<ImmidateActionDto>>> DataRecover(int id, string userId)
        {
            var result = await data.ImmAct.DataRecover(id,userId);
            return ApiResponse<IEnumerable<ImmidateActionDto>>.SuccessResponse(result);
        }
        public async Task<byte[]> Template()
        {
            var result = await data.ImmAct.Template();
            return result;
        }
        public async Task<IEnumerable<ImportResult>> Import(IFormFile file, string userId)
        {
            string filePath = Path.Combine(file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            //var result = await data.ImmAct.Import(filePath, userId);
            var ImportResult = await data.ImmAct.Import(filePath, userId);

            System.IO.File.Delete(filePath);

            //if (result.Contains("Invalid Data structure, please follow template format"))
            //{
            //    return new ApiResponse<IEnumerable<string>>
            //    {
            //        Success = false,
            //        Message = "Invalid Data structure, please follow template format",
            //        Content = null
            //    };
            //}

            var resultList = new List<ImportResult> { ImportResult };
            return resultList;
            //return ApiResponse<IEnumerable<string>>.SuccessResponse(result);
        }
    }
}
