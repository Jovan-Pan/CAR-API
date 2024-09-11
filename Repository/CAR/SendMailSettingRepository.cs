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

        public async Task<IEnumerable<MailSetiingDto>> GetSendMailSetting(string? search, string? ATsearchADV, string? ATDsearchADV)
        {

            string query;

            if (string.IsNullOrEmpty(search) && string.IsNullOrEmpty(ATsearchADV) && string.IsNullOrEmpty(ATDsearchADV))
            {
                query = SendMailSettingQuery.GetSendMailSetting;
            }
            else if(!string.IsNullOrEmpty(ATsearchADV) || !string.IsNullOrEmpty(ATDsearchADV))
            {
                query = SendMailSettingQuery.SearchadvData;
            }
            else
            {
                query = SendMailSettingQuery.SearchDatainDB;
            }
            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<MailSetiingDto>(query, new { search = search, ATsearchADV = ATsearchADV, ATDsearchADV = ATDsearchADV });

        }
        public async Task<IEnumerable<MailSetiingDto>> InsertNewSendMailSetting(string plant, string actiontype, string actiontypedesc, bool issendemail)
        {
            string query = string.Format(SendMailSettingQuery.InsertNewSendMailSetting);

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<MailSetiingDto>(query,new {plant = plant, actiontype = actiontype, actiontypedesc = actiontypedesc, issendemail = issendemail });

        }

        public async Task<IEnumerable<MailSetiingDto>> UpdateSendMailSetting(string actiontype, string actiontypedesc, bool issendemail)
        {
            string query = string.Format(SendMailSettingQuery.UpdateSendMailSetting);

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<MailSetiingDto>(query, new { actiontype = actiontype, actiontypedesc = actiontypedesc, issendemail = issendemail });

        }

        public async Task<IEnumerable<MailSetiingDto>> DataDelete(string actiontype)
        {
            string query = string.Format(SendMailSettingQuery.DataDelete);

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<MailSetiingDto>(query, new { actiontype = actiontype});

        }

        public async Task<IEnumerable<MailSetiingDto>> DataPermDelete(string actiontype)
        {
            string query = string.Format(SendMailSettingQuery.DataPermDelete);

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<MailSetiingDto>(query, new { actiontype = actiontype });

        }

        public async Task<IEnumerable<MailSetiingDto>> DataRecover(string actiontype)
        {
            string query = string.Format(SendMailSettingQuery.DataRecover);

            await using var conn = dbContext.CARConnection();
            return await conn.QueryAsync<MailSetiingDto>(query, new { actiontype = actiontype });

        }
    }
}
