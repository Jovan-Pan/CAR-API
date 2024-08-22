using Contracts.Repository.CAR;
using Dapper;
using Repository.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IssueSubmission
{
    internal sealed class IssueSubmissionRepository(DbContext dbContext): IIssueSubmissionRepository
    {
        public async Task<IEnumerable<string>> GetIssueStatus(string FormNo)
        {
            string query = IssueSubmissionQuery.GetIssueStatus;

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<string>(query, new { FormNo });
        }
    }
}
