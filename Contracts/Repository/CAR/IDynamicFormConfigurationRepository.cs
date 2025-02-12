using Entities.CAR;
using Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Repository.CAR
{
    public interface IDynamicFormConfigurationRepository
    {
        Task<IEnumerable<IssueFeedbackColumnInfo>> GetIssueFeedbackColumn();
        Task<IEnumerable<DynamicFormConfigurationDto>> GetTestingQueryResult(TestingQueryParam TestingQueryParam);
        Task<IEnumerable<DynamicFormConfigurationDto>> InsertNewData(DynamicFormConfigurationDto DynamicFormConfigurationDto);
        Task<IEnumerable<DynamicFormConfigurationDto>> GetDynamicFormConfiguration(string? search, bool delflag, string? formTypeAdv, string? fieldNameAdv, string? fieldTypeAdv, string? fieldLengthAdv, string? mandatoryAdv, string? fieldElementAdv, string? optionDataResourceAdv, string? dbResourceAdv, string? queryAdv, string? dataOptionAdv, string? sequenceAdv);
        Task<IEnumerable<DynamicFormConfigurationDto>> UpdateData(DynamicFormConfigurationDto DynamicFormConfigurationDto);
        Task<IEnumerable<DynamicFormConfigurationDto>> DataDelete(DynamicFormConfigurationDto DynamicFormConfigurationDto);
        Task<IEnumerable<DynamicFormConfigurationDto>> DataPermDelete(DynamicFormConfigurationDto DynamicFormConfigurationDto);
        Task<IEnumerable<DynamicFormConfigurationDto>> DataRecover(DynamicFormConfigurationDto DynamicFormConfigurationDto);
        Task<byte[]> Template();
        Task<ImportResult> Import(string filePath, string userId);
    }
}
