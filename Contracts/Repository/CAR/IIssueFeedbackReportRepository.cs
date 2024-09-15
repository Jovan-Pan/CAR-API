using Entities.CAR;
using Entities.MasterData;
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
        Task<TotalRecordForEachSttsDto> GetTotalRecordForEachStts(GetTotalRecordForEachSttsParam param, string condition);
        Task<IEnumerable<string>> GetFormNumberListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList);
        Task<IEnumerable<string>> GetDeptListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList);
        Task<IEnumerable<string>> GetprocecessGrpCodeFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList);
        Task<IEnumerable<string>> GetproductListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList);
        Task<IEnumerable<string>> GetModelListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList);
        Task<IEnumerable<string>> GetMatTypeListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList);
        Task<IEnumerable<string>> GetMaterialListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList);
        Task<IEnumerable<VendorDto>> GetVendorListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList);
    }
}
