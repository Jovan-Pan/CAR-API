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
    public interface IDynamicNewFormRepository
    {
        Task<IEnumerable<DynamicFormConfigurationDto>> GetDynamicFormConfiguration(bool delflag);
        Task<SqlConnection> OpenConnectionAsync();
        Task<string> GenerateNewFormNo(int plant, string FormType, SqlTransaction transaction);

        Task<string> GenerateNewFormNoWithVer(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> CreateNewIssueFeedBcakWithVers(string OldFormNumber, string NewFormNumber, string UserId, string UserName, SqlTransaction transaction);

        Task<int> InsertDataIssueFeedback(DynamicFormParameterDTO mydata, SqlTransaction transaction);
        Task<int> InsertDataAtchIssuer(DynamicFormParameterDTO mydata, SqlTransaction transaction);



    }
}
