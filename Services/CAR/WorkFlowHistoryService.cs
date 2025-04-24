using Contracts.Infrastructure;
using Contracts.Repository.MasterData;
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
using Entities.CAR;

namespace Services.CAR
{
    internal sealed class WorkFlowHistoryService(IDataManager data, IMDMRepository mdm, IMasterDataApi mdmP, ICacheManager memCache, ILocalizationService localization) : IWorkFlowHistoryService
    {
        public async Task<ApiResponse<IEnumerable<GetWorkFlowHistoryDto>>> GetWorkFlowHistory(GETWorkFlowHistory GETWorkFlowHistory)
        {
            var result = await data.WorkFlowHistory.GetWorkFlowHistory(GETWorkFlowHistory);
            return ApiResponse<IEnumerable<GetWorkFlowHistoryDto>>.SuccessResponse(result);
        }
    }
}
