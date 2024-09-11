using Entities.CAR;
using Entities.ParamRequest;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts.CAR
{
    public interface ISendMailSettingService
    {
        Task<ApiResponse<IEnumerable<MailSetiingDto>>> GetaData(SendMailSettingParam param);
        Task<ApiResponse<string>> sendemail(IssueSubmissionParameters mydata);
        Task<ApiResponse<IEnumerable<MailSetiingDto>>> GetSendMailSetting(string? search, string? ATsearchADV, string? ATDsearchADV);
        Task<ApiResponse<IEnumerable<MailSetiingDto>>> InsertNewSendMailSetting(string plant, string actiontype, string actiontypedesc, bool issendemail);
        Task<ApiResponse<IEnumerable<MailSetiingDto>>> UpdateSendMailSetting(string actiontype, string actiontypedesc, bool issendemail);
        Task<ApiResponse<IEnumerable<MailSetiingDto>>> DataDelete(string actiontype);
        Task<ApiResponse<IEnumerable<MailSetiingDto>>> DataPermDelete(string actiontype);
        Task<ApiResponse<IEnumerable<MailSetiingDto>>> DataRecover(string actiontype);
    }
}
