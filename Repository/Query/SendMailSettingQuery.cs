using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Query
{
    public class SendMailSettingQuery
    {
        public static readonly string GetaData = @"
        select 
        id,plant,actionType,actionTypeDesc,isSendEmail
        ,createdBy,createdByName,createdDate
        ,updatedBy,updatedByName,updatedDate
        from SendEmailSetting
        where plant = @plant 
        and (ActionType = @actionType or @actionType is null)
        ";
    }
}
