using Contracts.Repository.CAR;
using Dapper;
using Entities.CAR;
using Entities.MasterData;
using Entities.ParamRequest;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Server;
using Repository.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Repository.CAR
{
    internal sealed class IssueFeedbackReportRepository(DbContext dbContext) : IIssueFeedbackReportRepository
    {
        public async Task<int> GetTotalRecord(GlobalParam param, ConditionParams ConditionParams) {
            string query = string.Format(IssueFeedbackReportQuery.GetTotalRecord);
            query += Environment.NewLine;
            query += ConditionParams.ExtraWhereCondition;
            query += Environment.NewLine;


            await using var conn = dbContext.CARConnection();
            return await conn.ExecuteScalarAsync<int>(query, new { plant = param.Plant });
        }

        public async Task<IEnumerable<IssueFeedbackDto>> GetMaindata(GlobalParam param, ConditionParams ConditionParams)
        {
            string query = string.Format(IssueFeedbackReportQuery.GetMainData);
            query += Environment.NewLine;
            query += ConditionParams.ExtraWhereCondition;
            query += Environment.NewLine;
            query += ConditionParams.OrderByCondition;
            query += Environment.NewLine;
            query += IssueFeedbackReportQuery.skiprow;

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<IssueFeedbackDto>(query, 
                new { 
                    plant = param.Plant,
                    deptAuthList = param.deptAuthList,
                    productAuthList = param.productAuthList,
                    formType = param.formType,
                    formNumber = param.formNumber,
                    status = param.status,
                    mainStatus = param.mainStatus,
                    dept = param.dept,
                    processGrp = param.processGrp,
                    statusOfFinding = param.statusOfFinding,
                    product = param.product,
                    model = param.model,
                    mattype = param.mattype,
                    material = param.material,
                    vendor = param.vendor,
                    UserVendor = param.UserVendor,
                    fromdate = param.fromdate?.ToString("yyyy-MM-dd"),
                    todate = param.todate?.ToString("yyyy-MM-dd"),
                    skip = ConditionParams.skip, 
                    take = ConditionParams.take 
                });
        }

        public async Task<IEnumerable<string>> getIssuerId(int plant, string FormNo)
        {
            string query;
            if (FormNo.StartsWith("NCR") || FormNo.StartsWith("QFR"))
            {
                query = string.Format(IssueFeedbackReportQuery.getIssuerIdForNCR);
            }
            else
            {
                query = string.Format(IssueFeedbackReportQuery.getIssuerId);
            }
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<string>(query,new {plant ,FormNo});
        }

        public async Task<IEnumerable<string>> GetMailtocc(string formno)
        {
            string Processquery = IssueFeedbackReportQuery.GetMailtocc;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<string>(Processquery, new { formno });
        }

        public async Task<IEnumerable<string>>GetMailToCCStatic(string formno, int UserPlant, string group)
        {
            string Processquery = IssueFeedbackReportQuery.GetMailToCCStatic;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<string>(Processquery, new { plant= UserPlant ,group =  group});
        }

        public async Task<IEnumerable<IssueFeedbackAtchmentDto>> GetDataAttchment(int plant, IEnumerable<string> FormNoList, SqlTransaction? transaction)
        {
            string query = string.Format(IssueFeedbackReportQuery.GetDataAttchment);
            if (transaction is null)
            {
                await using var conn = dbContext.CARConnection();
                return await conn.QueryAsync<IssueFeedbackAtchmentDto>(query, new { plant = plant, FormNo = FormNoList });
            }
            else
            {
                var conn = transaction.Connection;
                return await conn.QueryAsync<IssueFeedbackAtchmentDto>(query, new { plant = plant, FormNo = FormNoList }, transaction);
            }
            
        }

        public async Task<TotalRecordForEachSttsDto> GetTotalRecordForEachStts(GetTotalRecordForEachSttsParam param, string condition)
        {
            string query = string.Format(IssueFeedbackReportQuery.GetTotalRecordForEachStts, condition);
            await using var conn = dbContext.CARConnection();
            return await conn.QueryFirstOrDefaultAsync<TotalRecordForEachSttsDto>(query, param);
        }

        public async Task<IEnumerable<string>> GetFormNumberListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList)
        {
            string query = IssueFeedbackReportQuery.GetFormNumberListFilter;

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<string>(query, new { plant, deptAuthList, productAuthList });
        }

        public async Task<IEnumerable<string>> GetDeptListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList)
        {
            string query = IssueFeedbackReportQuery.GetDeptListFilter;

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<string>(query, new { plant, deptAuthList, productAuthList });
        }

        public async Task<IEnumerable<string>> GetprocecessGrpCodeFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList)
        {
            string query = IssueFeedbackReportQuery.GetprocecessGrpCodeFilter;

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<string>(query, new { plant, deptAuthList, productAuthList });
        }

        public async Task<IEnumerable<string>> GetproductListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList)
        {
            string query = IssueFeedbackReportQuery.GetproductListFilter;

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<string>(query, new { plant, deptAuthList, productAuthList });
        }

        public async Task<IEnumerable<string>> GetModelListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList)
        {
            string query = IssueFeedbackReportQuery.GetModelListFilter;

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<string>(query, new { plant, deptAuthList, productAuthList });
        }

        public async Task<IEnumerable<string>> GetMatTypeListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList)
        {
            string query = IssueFeedbackReportQuery.GetMatTypeListFilter;

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<string>(query, new { plant, deptAuthList, productAuthList });
        }

        public async Task<IEnumerable<string>> GetMaterialListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList)
        {
            string query = IssueFeedbackReportQuery.GetMaterialListFilter;

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<string>(query, new { plant, deptAuthList, productAuthList });
        }

        public async Task<IEnumerable<VendorDto>> GetVendorListFilter(int plant, IEnumerable<string> deptAuthList, IEnumerable<string> productAuthList)
        {
            string query = IssueFeedbackReportQuery.GetVendorListFilter;

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<VendorDto>(query, new { plant, deptAuthList, productAuthList });
        }

        public async Task<int> ProcessUpdate(IssueFeedbacReportUpdateParam mydata, SqlTransaction transaction)
        {
            string query = IssueFeedbackReportQuery.ProcessUpdate;
            var conn = transaction.Connection;
            return await conn.ExecuteAsync(query, mydata, transaction);
        }

        public async Task<IEnumerable<UsrDto>> GetEmailRecipientsList(int plant, IEnumerable<string> FormNoList, SqlTransaction? transaction)
        {
            string query = IssueFeedbackReportQuery.GetEmailRecipientsList;
            if (transaction is null)
            {
                await using var conn = dbContext.CARConnection();
                return await conn.QueryAsync<UsrDto>(query, new { plant = plant, FormNoList = FormNoList });
            }
            else
            {
                var conn = transaction.Connection;
                return await conn.QueryAsync<UsrDto>(query, new { plant = plant, FormNoList = FormNoList
                }, transaction);
            }

        }
    }
}
