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
        select distinct ImmidateName from immedieteActionCategory
        where DelFlag = 0 and Plant = @plant";

        public static readonly string GetImmidateAction = @"
        select distinct id,plant,ImmidateName,createdby,createdDate,UpdatedBy,UpdatedDate,DelFlag from immedieteActionCategory";

        public static readonly string SearchadvData = @"SELECT * FROM immedieteActionCategory WHERE ImmidateName LIKE '%' + @search + '%'";

        public static readonly string SearchDatainDB = @"SELECT * FROM immedieteActionCategory WHERE ImmidateName LIKE '%' + @search + '%'";

        public static readonly string InsertNewImmidateAction = @"INSERT INTO immedieteActionCategory ( plant,ImmidateName,CreatedBy,CreatedDate) VALUES (@plant,@ImmidateName,SUSER_SNAME(),GETDATE())";

        public static readonly string UpdateImmidateAction = @" UPDATE immedieteActionCategory
        SET ImmidateName =@ImmidateName,UpdatedBy=SUSER_SNAME(),UpdatedDate=GETDATE()
        WHERE ID=@id";

        public static readonly string DataDelete = @"UPDATE immedieteActionCategory SET DelFlag = 1, UpdatedBy = suser_sname(), UpdatedDate =getdate() WHERE ID = @id";

        public static readonly string DataPermDelete = @"DELETE FROM immedieteActionCategory WHERE ID = @id ";

        public static readonly string DataRecover = @"UPDATE immedieteActionCategory SET DelFlag = 0, UpdatedBy = suser_sname(), UpdatedDate =getdate() WHERE ID = @id";

        public static readonly string Import = @"UPDATE immedieteActionCategory 
                                                SET 
                                                    Plant = B.Plant, 
                                                    ImmidateName = B.[Immidate Name], 
                                                    UpdatedBy = UPPER(@UserId),
                                                    UpdatedDate = GETDATE(), 
                                                    Delflag = 0 
                                                FROM immedieteActionCategory A 
                                                INNER JOIN ##temp B 
                                                ON (A.ImmidateName = B.[Immidate Name])
                                                INSERT INTO immedieteActionCategory 
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
                                                        FROM immedieteActionCategory A 
                                                        WHERE A.ImmidateName = B.[Immidate Name]
                                                    )";
    }

}
