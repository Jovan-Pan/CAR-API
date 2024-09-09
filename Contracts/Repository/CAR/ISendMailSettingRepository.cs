using Entities.CAR;
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
    }
}
