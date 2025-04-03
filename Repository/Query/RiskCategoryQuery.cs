using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Query
{
    public class RiskCategoryQuery
    {
        public static readonly string GetRiskCategory = @"
        SELECT DISTINCT ID, Plant, RiskCategory, CreatedBy, CreatedByName, CreatedDate, UpdatedBy, UpdatedByName, UpdatedDate, DelFlag
        FROM RiskCategory
        WHERE plant = @plant ";

        public static readonly string SearchRiskCategory = @"
        SELECT DISTINCT ID, Plant, RiskCategory, CreatedBy, CreatedByName, CreatedDate, UpdatedBy, UpdatedByName, UpdatedDate, DelFlag
        FROM RiskCategory
        WHERE plant = @plant and RiskCategory LIKE '%' + @SearchADV + '%'";

        public static readonly string SearchRiskCategoryInDB = @"
        SELECT DISTINCT ID, Plant, RiskCategory, CreatedBy, CreatedByName, CreatedDate, UpdatedBy, UpdatedByName, UpdatedDate, DelFlag
        FROM RiskCategory
        WHERE plant = @plant and RiskCategory LIKE '%' + @search + '%'";

        public static readonly string InsertNewData = @"
        INSERT INTO RiskCategory (Plant,RiskCategory, CreatedBy, CreatedByName, CreatedDate) 
        VALUES (@Plant,@RiskCategory, @userId, @userId, GETDATE())";

        public static readonly string UpdateData = @"
        UPDATE RiskCategory
        SET RiskCategory = @RiskCategory, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE()
        WHERE ID = @id";

        public static readonly string DeleteData = @"
        UPDATE RiskCategory
        SET DelFlag = 1, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE() 
        WHERE ID = @id";

        public static readonly string PermDeleteData = @"DELETE FROM RiskCategory WHERE ID = @id";

        public static readonly string RecoverData = @"
        UPDATE RiskCategory 
        SET DelFlag = 0, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE() 
        WHERE ID = @id";

        public static readonly string Import = @"
        UPDATE RiskCategory
        SET 
            Plant = B.Plant,
            RiskCategory = B.RiskCategory,
            UpdatedBy = UPPER(@userId),
            UpdatedByName = UPPER(@userId),
            UpdatedDate = GETDATE(),
            DelFlag = 0
        FROM RiskCategory A
        INNER JOIN ##temp B 
        ON (A.RiskCategory = B.RiskCategory)

        INSERT INTO RiskCategory 
        (Plant,RiskCategory, CreatedBy, CreatedByName, CreatedDate, DelFlag) 
        SELECT 
            B.Plant,
            B.RiskCategory, 
            UPPER(@userId),
            UPPER(@userId),
            GETDATE(),
            0
        FROM ##temp B
        WHERE NOT EXISTS (
            SELECT A.RiskCategory
            FROM RiskCategory A 
            WHERE A.RiskCategory = B.RiskCategory AND A.Plant = B.Plant
        )";

    }
}
