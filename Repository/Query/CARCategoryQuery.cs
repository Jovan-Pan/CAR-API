using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Query
{
    public class CARCategoryQuery
    {
        public static readonly string GetCARCategory = @"
        SELECT DISTINCT ID, Plant, CARCategory, CreatedBy, CreatedByName, CreatedDate, UpdatedBy, UpdatedByName, UpdatedDate, DelFlag
        FROM CARCategory
        WHERE 1=1";

        public static readonly string SearchCARCategory = @"
        SELECT DISTINCT ID, Plant, CARCategory, CreatedBy, CreatedByName, CreatedDate, UpdatedBy, UpdatedByName, UpdatedDate, DelFlag
        FROM CARCategory
        WHERE CARCategory LIKE '%' + @SearchADV + '%'";

        public static readonly string SearchCARCategoryInDB = @"
        SELECT DISTINCT ID, Plant, CARCategory, CreatedBy, CreatedByName, CreatedDate, UpdatedBy, UpdatedByName, UpdatedDate, DelFlag
        FROM CARCategory
        WHERE CARCategory LIKE '%' + @search + '%'";

        public static readonly string InsertNewCARCategory = @"
        INSERT INTO CARCategory (CARCategory, Plant, CreatedBy, CreatedByName, CreatedDate) 
        VALUES (@CARCategory, @Plant, @userId, @userId, GETDATE())";    

        public static readonly string UpdateCARCategory = @"
        UPDATE CARCategory
        SET CARCategory = @CARCategory, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE()
        WHERE ID = @id";

        public static readonly string DeleteCARCategory = @"
        UPDATE CARCategory
        SET DelFlag = 1, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE() 
        WHERE ID = @id";

        public static readonly string PermDeleteCARCategory = @"DELETE FROM CARCategory WHERE ID = @id";

        public static readonly string RecoverCARCategory = @"
        UPDATE CARCategory 
        SET DelFlag = 0, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE() 
        WHERE ID = @id";

        public static readonly string ImportCARCategory = @"
        UPDATE CARCategory
        SET 
            CARCategory = B.CARCategory,
            Plant = B.Plant,
            UpdatedBy = UPPER(@userId),
            UpdatedByName = UPPER(@userId),
            UpdatedDate = GETDATE(),
            DelFlag = 0
        FROM CARCategory A
        INNER JOIN ##temp B 
        ON (A.CARCategory = B.CARCategory)

        INSERT INTO CARCategory 
        (CARCategory, Plant, CreatedBy, CreatedByName, CreatedDate, DelFlag) 
        SELECT 
            B.CARCategory, 
            B.Plant,
            UPPER(@userId),
            UPPER(@userId),
            GETDATE(),
            0
        FROM ##temp B
        WHERE NOT EXISTS (
            SELECT A.CARCategory
            FROM CARCategory A 
            WHERE A.CARCategory = B.CARCategory
        )";
    }
}
