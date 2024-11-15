using Entities.CAR;
using Entities.MasterData;
using Entities.ParamRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Repository.CAR
{
    public interface ISendMailSettingRepository
    {
        Task<IEnumerable<MailSetiingDto>> GetaData(SendMailSettingParam param);
        Task<IEnumerable<MailSetiingDto>> GetSendMailSetting(string? search, string? ATsearchADV, string? ATDsearchADV, bool delflag);
        Task<IEnumerable<MailSetiingDto>> InsertNewSendMailSetting(string plant, string actiontype, string actiontypedesc, bool issendemail, string userId);
        Task<IEnumerable<MailSetiingDto>> UpdateSendMailSetting(string actiontype, string actiontypedesc, bool issendemail, string userId);
        Task<IEnumerable<MailSetiingDto>> DataDelete(string actiontype, string userId);
        Task<IEnumerable<MailSetiingDto>> DataPermDelete(string actiontype);
        Task<IEnumerable<MailSetiingDto>> DataRecover(string actiontype, string userId);
        Task<byte[]> Template();
        Task<ImportResult> Import(string filePath, string userId);
    }
}
