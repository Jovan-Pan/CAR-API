using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Query
{
    public class PossibleHazardsQuery
    {
        public static readonly string GetPossibleHazards = @"
        SELECT DISTINCT ID, Plant, PossibleHazards, CreatedBy, CreatedByName, CreatedDate, UpdatedBy, UpdatedByName, UpdatedDate, DelFlag
        FROM PossibleHazards
        WHERE Plant =@Plant";

        public static readonly string SearchPossibleHazards = @"
        SELECT DISTINCT ID, Plant, PossibleHazards, CreatedBy, CreatedByName, CreatedDate, UpdatedBy, UpdatedByName, UpdatedDate, DelFlag
        FROM PossibleHazards
        WHERE Plant =@Plant AND PossibleHazards LIKE '%' + @SearchADV + '%'";

        public static readonly string SearchPossibleHazardsInDB = @"
        SELECT DISTINCT ID, Plant, PossibleHazards, CreatedBy, CreatedByName, CreatedDate, UpdatedBy, UpdatedByName, UpdatedDate, DelFlag
        FROM PossibleHazards
        WHERE Plant =@Plant AND PossibleHazards LIKE '%' + @search + '%'";

        public static readonly string InsertNewData = @"
        INSERT INTO PossibleHazards (Plant,PossibleHazards, CreatedBy, CreatedByName, CreatedDate) 
        VALUES (@plant,@PossibleHazards, @userId, @userId, GETDATE())";

        public static readonly string UpdateData = @"
        UPDATE PossibleHazards
        SET PossibleHazards = @PossibleHazards, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE()
        WHERE ID = @id";

        public static readonly string DeleteData = @"
        UPDATE PossibleHazards
        SET DelFlag = 1, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE() 
        WHERE ID = @id";

        public static readonly string PermDeleteData = @"
        DELETE FROM PossibleHazards WHERE ID = @id";

        public static readonly string RecoverData = @"
        UPDATE PossibleHazards 
        SET DelFlag = 0, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE() 
        WHERE ID = @id";

        public static readonly string Import = @"
        UPDATE PossibleHazards
        SET 
            Plant = B.Plant,
            PossibleHazards = B.PossibleHazards,
            UpdatedBy = UPPER(@userId),
            UpdatedByName = UPPER(@userId),
            UpdatedDate = GETDATE(),
            DelFlag = 0
        FROM PossibleHazards A
        INNER JOIN ##temp B 
        ON (A.PossibleHazards = B.PossibleHazards)

        INSERT INTO PossibleHazards 
        (Plant,PossibleHazards, CreatedBy, CreatedByName, CreatedDate, DelFlag) 
        SELECT
            B.Plant,
            B.PossibleHazards, 
            UPPER(@userId),
            UPPER(@userId),
            GETDATE(),
            0
        FROM ##temp B
        WHERE NOT EXISTS (
            SELECT A.PossibleHazards
            FROM PossibleHazards A 
            WHERE A.PossibleHazards = B.PossibleHazards
        )";
    }
}
