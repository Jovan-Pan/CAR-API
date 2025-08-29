using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
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

        public static readonly string GetSendMailSetting = @" 
        select distinct id,plant,actionType,actionTypeDesc,isSendEmail,isDeleted,createdBy,createdByName,createdDate,updatedBy,updatedByName,updatedDate
        from SendEmailSetting where plant =@plant ";

        public static readonly string SearchDatainDB = @"
        SELECT distinct id,plant,actionType,actionTypeDesc,isSendEmail,isDeleted,createdBy,createdByName,createdDate,updatedBy,updatedByName,updatedDate
        FROM SendEmailSetting
        WHERE plant =@plant and (actionType LIKE '%' + @search + '%' OR actionTypeDesc LIKE '%' + @search + '%')";

        public static readonly string SearchadvData = @"
        SELECT distinct id,plant,actionType,actionTypeDesc,isSendEmail,isDeleted,createdBy,createdByName,createdDate,updatedBy,updatedByName,updatedDate
        FROM SendEmailSetting
        WHERE plant =@plant and (actionType LIKE '%' + @ATsearchADV + '%' OR actionTypeDesc LIKE '%' + @ATDsearchADV + '%')";

        public static readonly string InsertNewSendMailSetting = @"
        INSERT INTO SendEmailSetting ( plant,ActionType,ActionTypeDesc,isSendEmail,CreatedBy,CreatedByName,CreatedDate)
        VALUES (@plant,@ActionType,@ActionTypeDesc,@isSendEmail,@UserId,@UserId,GETDATE())";

        public static readonly string UpdateSendMailSetting = @" 
        UPDATE SendEmailSetting
        SET ActionTypeDesc = @ActionTypeDesc , isSendEmail = @issendemail , UpdatedBy=@UserId, UpdatedByName=@UserId, UpdatedDate=GETDATE()
        WHERE ActionType =@ActionType";

        public static readonly string DataDelete = @"
        UPDATE SendEmailSetting
        SET isDeleted = 1, UpdatedBy = @UserId, UpdatedByName=@UserId, UpdatedDate =getdate() 
        WHERE ActionType =@ActionType";

        public static readonly string DataPermDelete = @"DELETE FROM SendEmailSetting WHERE ActionType =@ActionType ";

        public static readonly string DataRecover = @"
        UPDATE SendEmailSetting 
        SET isDeleted = 0, UpdatedBy = @UserId, UpdatedByName=@UserId, UpdatedDate =getdate() 
        WHERE ActionType =@ActionType";

        public static readonly string Import = @"
                                                UPDATE SendEmailSetting
                                                SET
                                                    Plant = B.plant,
                                                    ActionTypeDesc = B.[Action Type Desc],
                                                    isSendEmail = B.isSendEmail,
                                                    UpdatedBy = UPPER(@UserId),
                                                    UpdatedByName = UPPER(@UserId),
                                                    UpdatedDate = GETDATE(),
                                                    isDeleted = 0
                                                FROM SendEmailSetting A
                                                INNER JOIN ##temp B 
                                                    ON A.ActionType = B.[Action Type] AND A.Plant = B.Plant

                                                INSERT INTO SendEmailSetting
                                                    (Plant, ActionType, ActionTypeDesc, isSendEmail, CreatedBy, CreatedByName, CreatedDate, isDeleted)
                                                SELECT
                                                    B.plant,
                                                    B.[Action Type],
                                                    B.[Action Type Desc],
                                                    B.isSendEmail,
                                                    UPPER(@UserId),
                                                    UPPER(@UserId),
                                                    GETDATE(),
                                                    0 
                                                FROM ##temp B
                                                WHERE NOT EXISTS (
                                                    SELECT A.ActionType
                                                    FROM SendEmailSetting A
                                                    WHERE A.ActionType = B.[Action Type] AND A.Plant = B.Plant
                                                )";
    }
}