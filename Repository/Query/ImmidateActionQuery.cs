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
        select distinct ImmidateName from immedieteAction
        where DelFlag = 0 and Plant = @plant";

        public static readonly string GetImmidateAction = @"
        select distinct id,plant,ImmidateName,createdby,createdDate,UpdatedBy,UpdatedDate,DelFlag from immedieteAction";

        public static readonly string SearchadvData = @"SELECT * FROM immedieteAction WHERE ImmidateName LIKE '%' + @SearchADV + '%'";

        public static readonly string SearchDatainDB = @"SELECT * FROM immedieteAction WHERE ImmidateName LIKE '%' + @search + '%'";

        public static readonly string InsertNewImmidateAction = @"INSERT INTO immedieteAction ( plant,ImmidateName,CreatedBy,CreatedDate) VALUES (@plant,@ImmidateName,@userId,GETDATE())";

        public static readonly string UpdateImmidateAction = @" UPDATE immedieteAction
        SET ImmidateName =@ImmidateName,UpdatedBy=@userId,UpdatedDate=GETDATE()
        WHERE ID=@id";

        public static readonly string DataDelete = @"UPDATE immedieteAction SET DelFlag = 1, UpdatedBy = @userId, UpdatedDate =getdate() WHERE ID = @id";

        public static readonly string DataPermDelete = @"DELETE FROM immedieteAction WHERE ID = @id ";

        public static readonly string DataRecover = @"UPDATE immedieteAction SET DelFlag = 0, UpdatedBy = @userId, UpdatedDate =getdate() WHERE ID = @id";

        public static readonly string Import = @"UPDATE immedieteAction 
                                                SET 
                                                    Plant = B.Plant, 
                                                    ImmidateName = B.[Immidate Name], 
                                                    UpdatedBy = UPPER(@UserId),
                                                    UpdatedDate = GETDATE(), 
                                                    Delflag = 0 
                                                FROM immedieteAction A 
                                                INNER JOIN ##temp B 
                                                ON (A.ImmidateName = B.[Immidate Name])
                                                INSERT INTO immedieteAction 
                                                (Plant, ImmidateName, CreatedBy, CreatedDate, Delflag) 
                                                SELECT 
                                                    B.Plant,
                                                    UPPER([Immidate Name]), 
                                                    UPPER(@UserId),
                                                    GETDATE(), 
                                                    0 
                                                FROM ##temp B
                                                WHERE 
                                                    NOT EXISTS (
                                                        SELECT A.ImmidateName
                                                        FROM immedieteAction A 
                                                        WHERE A.ImmidateName = B.[Immidate Name]
                                                    )";
    }

}
