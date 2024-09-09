using Entities.CAR;
using Entities.ParamRequest;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts.CAR
{
    public interface ISendMailSettingService
    {
        Task<ApiResponse<IEnumerable<MailSetiingDto>>> GetaData(SendMailSettingParam param);
        Task<ApiResponse<string>> sendemail(IssueSubmissionParameters mydata);
    }
}
