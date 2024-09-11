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

        public static readonly string GetSendMailSetting = @" select 
        id,plant,actionType,actionTypeDesc,isSendEmail
        ,createdBy,createdByName,createdDate
        ,updatedBy,updatedByName,updatedDate
        from SendEmailSetting";

        public static readonly string SearchDatainDB = @"SELECT * FROM SendEmailSetting WHERE actionType LIKE '%' + @search + '%' OR actionTypeDesc LIKE '%' + @search + '%'";

        public static readonly string SearchadvData = @"SELECT * FROM SendEmailSetting WHERE actionType LIKE '%' + @ATsearchADV + '%' OR actionTypeDesc LIKE '%' + @ATDsearchADV + '%'";

        public static readonly string InsertNewSendMailSetting = @"INSERT INTO SendEmailSetting ( plant,ActionType,ActionTypeDesc,isSendEmail,CreatedBy,CreatedByName,CreatedDate) VALUES (@plant,@ActionType,@ActionTypeDesc,@isSendEmail,SUSER_SNAME(),HOST_name(),GETDATE())";

        public static readonly string UpdateSendMailSetting = @" UPDATE SendEmailSetting
        SET ActionTypeDesc = @ActionTypeDesc , isSendEmail = @issendemail , UpdatedBy=SUSER_SNAME(), UpdatedByName=HOST_name(), UpdatedDate=GETDATE()
        WHERE ActionType =@ActionType";

        public static readonly string DataDelete = @"UPDATE SendEmailSetting SET isDeleted = 1, UpdatedBy = suser_sname(), UpdatedByName=HOST_name(), UpdatedDate =getdate() WHERE ActionType =@ActionType";

        public static readonly string DataPermDelete = @"DELETE FROM SendEmailSetting WHERE ActionType =@ActionType ";

        public static readonly string DataRecover = @"UPDATE SendEmailSetting SET isDeleted = 0, UpdatedBy = suser_sname(), UpdatedByName=HOST_name(), UpdatedDate =getdate() WHERE ActionType =@ActionType";
    }
}
