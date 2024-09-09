using Contracts.Repository.CAR;
using Dapper;
using Entities.CAR;
using Entities.ParamRequest;
using Repository.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.CAR
{
    internal sealed class SendMailSettingRepository(DbContext dbContext): ISendMailSettingRepository
    {
        public async Task<IEnumerable<MailSetiingDto>> GetaData(SendMailSettingParam param)
        {
            string query = string.Format(SendMailSettingQuery.GetaData);

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<MailSetiingDto>(query, param);

        }
    }
}
