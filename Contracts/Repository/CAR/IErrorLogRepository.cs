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
    public interface IErrorLogRepository
    {
        Task<(IEnumerable<ErrorLogModel> data, int totalCount)> ShowDataErrorLog(ErrorLogParam param, ConditionParams ConditionParams);
        Task<int> InsertDataToTERRORLOG(ErrorLogModel ErrorLogData, SqlTransaction? transaction = null);
    }
}
