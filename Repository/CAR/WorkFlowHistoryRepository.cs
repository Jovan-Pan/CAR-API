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
using System.Xml.Linq;

namespace Repository.CAR
{
    internal sealed class WorkFlowHistoryRepository(DbContext dbContext) : IWorkFlowHistoryRepository
    {
        public async Task<IEnumerable<GetWorkFlowHistoryDto>> GetWorkFlowHistory(GETWorkFlowHistory GETWorkFlowHistory)
        {
            if (GETWorkFlowHistory.formno == "undefined")
            {
                GETWorkFlowHistory.formno = null;
            }
                string query = WorkFlowHistoryQuery.GetWorkFlowHistory;
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<GetWorkFlowHistoryDto>(query, new { formno = GETWorkFlowHistory.formno, plant = GETWorkFlowHistory .Plant});
        }
        public async Task<IEnumerable<IssueSubmissionParameters>> InsertNewData(IssueSubmissionParameters mydata, SqlTransaction transaction)
        {
            string query = WorkFlowHistoryQuery.InsertNewData;
            await using var conn = dbContext.CARConnection();
            if (mydata.buttonText == "Approve")
            {
                return await conn.QueryAsync<IssueSubmissionParameters>(query, new { plant = mydata.UserPlant, role = mydata.mailactionType, status = mydata.FlowStatus, formno = mydata.FormNumber, performedBy = mydata.UserName, decision = mydata.buttonText, comment = mydata.Comment });
            }
            else
            {
                return await conn.QueryAsync<IssueSubmissionParameters>(query, new { plant = mydata.UserPlant, role = mydata.mailactionType, status = mydata.FlowStatus, formno = mydata.FormNumber, performedBy = mydata.UserName, decision = mydata.buttonText, comment = mydata.rejectReason });
            }
        }
    }
}
