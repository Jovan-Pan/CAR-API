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
    public interface IIssueFeedbackReportRepository
    {
        Task<int> GetTotalRecord(GlobalParam param, ConditionParams ConditionParams);
        Task<IEnumerable<IssueFeedbackDto>> GetMaindata(GlobalParam param, ConditionParams ConditionParams);
        Task<IEnumerable<IssueFeedbackAtchmentDto>> GetDataAttchment(int plant, IEnumerable<string> FormNoList, SqlTransaction? transaction);
    }
}
