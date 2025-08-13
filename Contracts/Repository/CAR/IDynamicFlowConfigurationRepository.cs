using Entities.CAR;
using Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Repository.CAR
{
    public interface IDynamicFlowConfigurationRepository
    {
        Task<IEnumerable<DynamicFlowConfigurationDto>> GetDynamicFlowConfiguration(string search, string SearchFTADV, string SearchADV, bool delflag, int plant);
        Task<IEnumerable<DynamicFlowConfigurationDto>> InsertNewData(CRUDDynamicFlowConfigurationDto CRUDDynamicFlowConfigurationDto);
        Task<IEnumerable<DynamicFlowConfigurationDto>> UpdateData(CRUDDynamicFlowConfigurationDto CRUDDynamicFlowConfigurationDto);
        Task<IEnumerable<DynamicFlowConfigurationDto>> DataDelete(CRUDDynamicFlowConfigurationDto CRUDDynamicFlowConfigurationDto);
        Task<IEnumerable<DynamicFlowConfigurationDto>> DataPermDelete(CRUDDynamicFlowConfigurationDto CRUDDynamicFlowConfigurationDto);
        Task<IEnumerable<DynamicFlowConfigurationDto>> DataRecover(CRUDDynamicFlowConfigurationDto CRUDDynamicFlowConfigurationDto);
        Task<byte[]> Template();
        Task<ImportResult> Import(string filePath, string userId);
    }
}
