using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.CAR;
using Entities.MasterData;
using Microsoft.AspNetCore.Http;

namespace Services.Contracts.CAR
{
    public interface IDynamicFormConfigurationService
    {
        Task<ApiResponse<IEnumerable<IssueFeedbackColumnInfo>>> GetIssueFeedbackColumn();
        Task<ApiResponse<IEnumerable<DynamicFormConfigurationDto>>> GetTestingQueryResult(TestingQueryParam TestingQueryParam);
        Task<ApiResponse<IEnumerable<DynamicFormConfigurationDto>>> InsertNewData(DynamicFormConfigurationDto DynamicFormConfigurationDto);
        Task<ApiResponse<IEnumerable<DynamicFormConfigurationDto>>> GetDynamicFormConfiguration(string? search, bool delflag, string? formTypeAdv, string? fieldNameAdv, string? fieldTypeAdv, string? fieldLengthAdv, string? mandatoryAdv, string? fieldElementAdv, string? optionDataResourceAdv, string? dbResourceAdv, string? queryAdv, string? dataOptionAdv, string? sequenceAdv);
        Task<ApiResponse<IEnumerable<DynamicFormConfigurationDto>>> UpdateData(DynamicFormConfigurationDto DynamicFormConfigurationDto);
        Task<ApiResponse<IEnumerable<DynamicFormConfigurationDto>>> DataDelete(DynamicFormConfigurationDto DynamicFormConfigurationDto);
        Task<ApiResponse<IEnumerable<DynamicFormConfigurationDto>>> DataPermDelete(DynamicFormConfigurationDto DynamicFormConfigurationDto);
        Task<ApiResponse<IEnumerable<DynamicFormConfigurationDto>>> DataRecover(DynamicFormConfigurationDto DynamicFormConfigurationDto);
        Task<byte[]> Template();
        Task<IEnumerable<ImportResult>> Import(IFormFile file, string userId);
    }
}
