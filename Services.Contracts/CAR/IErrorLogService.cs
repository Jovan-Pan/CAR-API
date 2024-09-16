using Entities;
using Entities.CAR;
using Entities.MasterData;
using Entities.ParamRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts.CAR
{
    public interface IErrorLogService
    {
        Task<int> SaveErrorLog(Exception exdb, string userLogin);
        Task<PagedResponse> ShowDataErrorLog(GlobalParam param);
        Task<ApiResponse<IEnumerable<ErrorLogModel>>> GetDataReport(string startdate, string enddate);

    }
}
