using Contracts.Infrastructure;
using Contracts.Repository;
using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Contracts.CAR;
using Entities.MasterData;
using Entities;
using Microsoft.AspNetCore.Http;
using Entities.CAR;

namespace Services.CAR
{
    internal sealed class DynamicFlowConfigurationService(IDataManager data, ICacheManager memCache, ILocalizationService localization) : IDynamicFlowConfigurationService
    {
        public async Task<ApiResponse<IEnumerable<DynamicFlowConfigurationDto>>> GetDynamicFlowConfiguration(string search, string SearchFTADV, string SearchADV, bool delflag)
        {
            var result = await data.DynamicFlowConfiguration.GetDynamicFlowConfiguration(search, SearchFTADV, SearchADV, delflag);
            return ApiResponse<IEnumerable<DynamicFlowConfigurationDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<DynamicFlowConfigurationDto>>> InsertNewData(CRUDDynamicFlowConfigurationDto CRUDDynamicFlowConfigurationDto)
        {
            var result = await data.DynamicFlowConfiguration.InsertNewData(CRUDDynamicFlowConfigurationDto);
            return ApiResponse<IEnumerable<DynamicFlowConfigurationDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<DynamicFlowConfigurationDto>>> UpdateData(CRUDDynamicFlowConfigurationDto CRUDDynamicFlowConfigurationDto)
        {
            var result = await data.DynamicFlowConfiguration.UpdateData(CRUDDynamicFlowConfigurationDto);
            return ApiResponse<IEnumerable<DynamicFlowConfigurationDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<DynamicFlowConfigurationDto>>> DataDelete(CRUDDynamicFlowConfigurationDto CRUDDynamicFlowConfigurationDto)
        {
            var result = await data.DynamicFlowConfiguration.DataDelete(CRUDDynamicFlowConfigurationDto);
            return ApiResponse<IEnumerable<DynamicFlowConfigurationDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<DynamicFlowConfigurationDto>>> DataPermDelete(CRUDDynamicFlowConfigurationDto CRUDDynamicFlowConfigurationDto)
        {
            var result = await data.DynamicFlowConfiguration.DataPermDelete(CRUDDynamicFlowConfigurationDto);
            return ApiResponse<IEnumerable<DynamicFlowConfigurationDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<DynamicFlowConfigurationDto>>> DataRecover(CRUDDynamicFlowConfigurationDto CRUDDynamicFlowConfigurationDto)
        {
            var result = await data.DynamicFlowConfiguration.DataRecover(CRUDDynamicFlowConfigurationDto);
            return ApiResponse<IEnumerable<DynamicFlowConfigurationDto>>.SuccessResponse(result);
        }

        public async Task<byte[]> Template()
        {
            var result = await data.DynamicFlowConfiguration.Template();
            return result;
        }

        public async Task<IEnumerable<ImportResult>> Import(IFormFile file, string userId)
        {
            string filePath = Path.Combine(file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var ImportResult = await data.DynamicFlowConfiguration.Import(filePath, userId);

            var resultList = new List<ImportResult> { ImportResult };
            return resultList;
        }
    }
}
