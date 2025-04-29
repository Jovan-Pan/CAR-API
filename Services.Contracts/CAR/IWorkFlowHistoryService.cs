using Entities.MasterData;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.CAR;

namespace Services.Contracts.CAR
{
    public interface IWorkFlowHistoryService
    {
        Task<ApiResponse<IEnumerable<GetWorkFlowHistoryDto>>> GetWorkFlowHistory(GETWorkFlowHistory GETWorkFlowHistory);
    }
}
