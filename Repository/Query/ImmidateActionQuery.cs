using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Query
{
    public class ImmidateActionQuery
    {
        public static readonly string getImmidateActionList = @"
        select distinct ImmidateName from ImmidateAction
        where DelFlag = 0 and Plant = @plant";

        public static readonly string GetImmidateAction = @"
        select distinct id,plant,ImmidateName,createdby,createdDate,UpdatedBy,UpdatedDate,DelFlag
        from ImmidateAction WHERE plant =@plant ";

        public static readonly string SearchadvData = @"
        SELECT distinct id,plant,ImmidateName,createdby,createdDate,UpdatedBy,UpdatedDate,DelFlag
        FROM ImmidateAction 
        WHERE plant =@plant and ImmidateName LIKE '%' + @SearchADV + '%'";

        public static readonly string SearchDatainDB = @"
        SELECT distinct id,plant,ImmidateName,createdby,createdDate,UpdatedBy,UpdatedDate,DelFlag
        FROM ImmidateAction 
        WHERE plant =@plant and ImmidateName LIKE '%' + @search + '%'";   

        public static readonly string InsertNewImmidateAction = @"
        INSERT INTO ImmidateAction ( plant,ImmidateName,CreatedBy,CreatedDate) 
        VALUES (@plant,@ImmidateName,@userId,GETDATE())";

        public static readonly string UpdateImmidateAction = @" 
        UPDATE ImmidateAction
        SET ImmidateName =@ImmidateName,UpdatedBy=@userId,UpdatedDate=GETDATE()
        WHERE ID=@id";

        public static readonly string DataDelete = @"
        UPDATE ImmidateAction
        SET DelFlag = 1, UpdatedBy = @userId, UpdatedDate =getdate() 
        WHERE ID = @id";

        public static readonly string DataPermDelete = @"DELETE FROM ImmidateAction WHERE ID = @id ";

        public static readonly string DataRecover = @"
        UPDATE ImmidateAction 
        SET DelFlag = 0, UpdatedBy = @userId, UpdatedDate =getdate() 
        WHERE ID = @id";

        public static readonly string Import = @"UPDATE ImmidateAction 
                                                SET 
                                                    Plant = B.Plant, 
                                                    ImmidateName = B.[Immidate Name], 
                                                    UpdatedBy = UPPER(@UserId),
                                                    UpdatedDate = GETDATE(), 
                                                    Delflag = 0 
                                                FROM ImmidateAction A 
                                                INNER JOIN ##temp B 
                                                ON (A.ImmidateName = B.[Immidate Name] AND A.Plant = B.Plant)
                                                INSERT INTO ImmidateAction 
                                                (Plant, ImmidateName, CreatedBy, CreatedDate, Delflag) 
                                                SELECT 
                                                    B.Plant,
                                                    B.[Immidate Name], 
                                                    UPPER(@UserId),
                                                    GETDATE(), 
                                                    0 
                                                FROM ##temp B
                                                WHERE 
                                                    NOT EXISTS (
                                                        SELECT A.ImmidateName
                                                        FROM ImmidateAction A 
                                                        WHERE A.ImmidateName = B.[Immidate Name] AND A.Plant = B.Plant
                                                    )";
    }

}
