using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Query
{
    public class RootCauseQuery
    {
        public static readonly string getRootCauseList = @"
        select distinct RootCauseName from RootCauseCategory
        where DelFlag = 0 and Plant = @plant";

        public static readonly string InsertNewRootCauseName = @"INSERT INTO RootCauseCategory ( plant,RootCauseName,CreatedBy,CreatedDate) VALUES (2100,@RootCauseName,SUSER_SNAME(),GETDATE())";

        public static readonly string GetRootCauseCategory = @"
        select distinct id,RootCauseName,plant,RootCauseName,CreatedBy,CreatedDate,UpdatedBy,UpdatedDate,DelFlag from RootCauseCategory
        ";

        public static readonly string UpdateNewRootCauseCategory = @" UPDATE RootCauseCategory
        SET RootCauseName =@RootCauseName,UpdatedBy=SUSER_SNAME(),UpdatedDate=GETDATE()
        WHERE ID=@id";

        public static readonly string DataDelete = @"UPDATE rootcausecategory SET DelFlag = 1, UpdatedBy = suser_sname(), UpdatedDate =getdate() WHERE ID = @id";

        public static readonly string DataPermDelete = @"DELETE FROM rootcausecategory WHERE ID = @id ";

        public static readonly string DataRecover = @"UPDATE rootcausecategory SET DelFlag = 0, UpdatedBy = suser_sname(), UpdatedDate =getdate() WHERE ID = @id";

        public static readonly string SearchDatainDB = @"SELECT * FROM rootcausecategory WHERE RootCauseName LIKE '%' + @search + '%';";

        public static readonly string SearchadvData = @"SELECT * FROM rootcausecategory WHERE RootCauseName LIKE '%' + @SearchADV + '%';";

        public static readonly string Import = @"UPDATE rootcausecategory 
                                                SET 
                                                    Plant = B.Plant, 
                                                    RootCauseName = B.[Root Cause Name], 
                                                    UpdatedBy = UPPER(@UserId),
                                                    UpdatedDate = GETDATE(), 
                                                    Delflag = 0 
                                                FROM RootCauseCategory A 
                                                INNER JOIN ##temp B 
                                                ON (A.RootCauseName = B.[Root Cause Name])
                                                INSERT INTO rootcausecategory 
                                                (Plant, RootCauseName, CreatedBy, CreatedDate, Delflag) 
                                                SELECT 
                                                    B.Plant,
                                                    UPPER([Root Cause Name]), 
                                                    UPPER(@UserId),
                                                    GETDATE(), 
                                                    0 
                                                FROM ##temp B
                                                WHERE 
                                                    NOT EXISTS (
                                                        SELECT A.RootCauseName
                                                        FROM RootCauseCategory A 
                                                        WHERE A.RootCauseName = B.[Root Cause Name]
                                                    )";
    }
}
