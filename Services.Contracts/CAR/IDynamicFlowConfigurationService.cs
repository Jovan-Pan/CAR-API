using Entities.MasterData;
using Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.CAR;

namespace Services.Contracts.CAR
{
    public interface IDynamicFlowConfigurationService
    {
        Task<ApiResponse<IEnumerable<DynamicFlowConfigurationDto>>> GetDynamicFlowConfiguration(string? search, string? SearchFTADV, string? SearchADV, bool delflag);
        Task<ApiResponse<IEnumerable<DynamicFlowConfigurationDto>>> InsertNewData(CRUDDynamicFlowConfigurationDto CRUDDynamicFlowConfigurationDto);
        Task<ApiResponse<IEnumerable<DynamicFlowConfigurationDto>>> UpdateData(CRUDDynamicFlowConfigurationDto CRUDDynamicFlowConfigurationDto);
        Task<ApiResponse<IEnumerable<DynamicFlowConfigurationDto>>> DataDelete(CRUDDynamicFlowConfigurationDto CRUDDynamicFlowConfigurationDto);
        Task<ApiResponse<IEnumerable<DynamicFlowConfigurationDto>>> DataPermDelete(CRUDDynamicFlowConfigurationDto CRUDDynamicFlowConfigurationDto);
        Task<ApiResponse<IEnumerable<DynamicFlowConfigurationDto>>> DataRecover(CRUDDynamicFlowConfigurationDto CRUDDynamicFlowConfigurationDto);
        Task<byte[]> Template();
        Task<IEnumerable<ImportResult>> Import(IFormFile file, string userId);
    }
}
