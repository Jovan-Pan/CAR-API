using Contracts.Infrastructure;
using Contracts.Repository;
using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.ParamRequest;
using Services.Contracts.CAR;
using Entities.CAR;
using Entities.MasterData;
using Microsoft.AspNetCore.Http;

namespace Services.CAR
{
    internal sealed class DynamicFormConfigurationService(IDataManager data, ICacheManager memCache, ILocalizationService localization) : IDynamicFormConfigurationService
    {
        public async Task<ApiResponse<IEnumerable<IssueFeedbackColumnInfo>>> GetIssueFeedbackColumn()
        {
            var result = await data.DynamicFormConfiguration.GetIssueFeedbackColumn();
            return ApiResponse<IEnumerable<IssueFeedbackColumnInfo>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<DynamicFormConfigurationDto>>> GetTestingQueryResult(TestingQueryParam TestingQueryParam)
        {
            var result = await data.DynamicFormConfiguration.GetTestingQueryResult(TestingQueryParam);
            return ApiResponse<IEnumerable<DynamicFormConfigurationDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<DynamicFormConfigurationDto>>> InsertNewData(DynamicFormConfigurationDto DynamicFormConfigurationDto)
        {
            var result = await data.DynamicFormConfiguration.InsertNewData(DynamicFormConfigurationDto);
            return ApiResponse<IEnumerable<DynamicFormConfigurationDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<DynamicFormConfigurationDto>>> GetDynamicFormConfiguration(string? search, bool delflag, string? formTypeAdv, string? fieldNameAdv, string? fieldTypeAdv, string? fieldLengthAdv, string? mandatoryAdv, string? fieldElementAdv, string? optionDataResourceAdv, string? dbResourceAdv, string? queryAdv, string? dataOptionAdv, string? sequenceAdv)
        {
            var result = await data.DynamicFormConfiguration.GetDynamicFormConfiguration(search, delflag, formTypeAdv, fieldNameAdv, fieldTypeAdv, fieldLengthAdv, mandatoryAdv, fieldElementAdv, optionDataResourceAdv, dbResourceAdv, queryAdv, dataOptionAdv, sequenceAdv);
            return ApiResponse<IEnumerable<DynamicFormConfigurationDto>>.SuccessResponse(result);
        }
        public async Task<ApiResponse<IEnumerable<DynamicFormConfigurationDto>>> UpdateData(DynamicFormConfigurationDto DynamicFormConfigurationDto)
        {
            var result = await data.DynamicFormConfiguration.UpdateData(DynamicFormConfigurationDto);
            return ApiResponse<IEnumerable<DynamicFormConfigurationDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<DynamicFormConfigurationDto>>> DataDelete(DynamicFormConfigurationDto DynamicFormConfigurationDto)
        {
            var result = await data.DynamicFormConfiguration.DataDelete(DynamicFormConfigurationDto);
            return ApiResponse<IEnumerable<DynamicFormConfigurationDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<DynamicFormConfigurationDto>>> DataPermDelete(DynamicFormConfigurationDto DynamicFormConfigurationDto)
        {
            var result = await data.DynamicFormConfiguration.DataPermDelete(DynamicFormConfigurationDto);
            return ApiResponse<IEnumerable<DynamicFormConfigurationDto>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<IEnumerable<DynamicFormConfigurationDto>>> DataRecover(DynamicFormConfigurationDto DynamicFormConfigurationDto)
        {
            var result = await data.DynamicFormConfiguration.DataRecover(DynamicFormConfigurationDto);
            return ApiResponse<IEnumerable<DynamicFormConfigurationDto>>.SuccessResponse(result);
        }

        public async Task<byte[]> Template()
        {
            var result = await data.DynamicFormConfiguration.Template();
            return result;
        }

        public async Task<IEnumerable<ImportResult>> Import(IFormFile file, string userId)
        {
            string filePath = Path.Combine(file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var ImportResult = await data.DynamicFormConfiguration.Import(filePath, userId);

            var resultList = new List<ImportResult> { ImportResult };
            return resultList;
        }
    }
}
