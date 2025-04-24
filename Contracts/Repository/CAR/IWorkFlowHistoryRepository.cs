using Entities.CAR;
using Entities.ParamRequest;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Repository.CAR
{
    public interface IWorkFlowHistoryRepository
    {
        Task<IEnumerable<GetWorkFlowHistoryDto>> GetWorkFlowHistory(GETWorkFlowHistory GETWorkFlowHistory);
        Task<IEnumerable<IssueSubmissionParameters>> InsertNewData(IssueSubmissionParameters mydata, SqlTransaction transaction);
    }
}
