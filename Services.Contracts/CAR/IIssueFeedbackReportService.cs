using Entities.ParamRequest;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.CAR;

namespace Services.Contracts.CAR
{
    public interface IIssueFeedbackReportService
    {
        Task<ApiResponse<IssueFeedbackResultDto>> GetDataReport(GlobalParam param);
    }
}
