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

namespace Services.MasterData
{
    internal sealed class ImmidateActionService(IDataManager data, ICacheManager memCache, ILocalizationService localization): IImmidateActionService
    {
        public async Task<ApiResponse<IEnumerable<string>>> getImmidateActionList(int plant)
        {
            var result = await data.ImmAct.getImmidateActionList(plant);
            return ApiResponse<IEnumerable<string>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<ImmidateActionDto>>> GetImmidateAction(string? search, string? SearchADV)
        {
            var result = await data.ImmAct.GetImmidateAction(search, SearchADV);
            return ApiResponse<IEnumerable<ImmidateActionDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<ImmidateActionDto>>> InsertNewImmidateAction(string ImmidateName, int plant)
        {
            var result = await data.ImmAct.InsertNewImmidateAction(ImmidateName,plant);
            return ApiResponse<IEnumerable<ImmidateActionDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<ImmidateActionDto>>> UpdateImmidateAction(string ImmidateName, int id)
        {
            var result = await data.ImmAct.UpdateImmidateAction(ImmidateName, id);
            return ApiResponse<IEnumerable<ImmidateActionDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<ImmidateActionDto>>> DataDelete( int id)
        {
            var result = await data.ImmAct.DataDelete( id);
            return ApiResponse<IEnumerable<ImmidateActionDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<ImmidateActionDto>>> DataPermDelete(int id)
        {
            var result = await data.ImmAct.DataPermDelete(id);
            return ApiResponse<IEnumerable<ImmidateActionDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<ImmidateActionDto>>> DataRecover(int id)
        {
            var result = await data.ImmAct.DataRecover(id);
            return ApiResponse<IEnumerable<ImmidateActionDto>>.SuccessResponse(result);
        }
        public async Task<byte[]> Template()
        {
            var result = await data.ImmAct.Template();
            return result;
        }
    }
}
