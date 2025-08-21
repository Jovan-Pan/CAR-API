using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Query
{
    public class TypeofcontraventionQuery
    {
        public static readonly string GetTypeofcontravention = @"
        SELECT DISTINCT ID, Plant, TypeOfContravention, CreatedBy, CreatedByName, CreatedDate, UpdatedBy, UpdatedByName, UpdatedDate, DelFlag
        FROM TypeOfContravention
        WHERE plant = @plant ";

        public static readonly string SearchDataADV = @"
        SELECT DISTINCT ID, Plant, TypeOfContravention, CreatedBy, CreatedByName, CreatedDate, UpdatedBy, UpdatedByName, UpdatedDate, DelFlag
        FROM TypeOfContravention
        WHERE plant = @plant and TypeOfContravention LIKE '%' + @SearchADV + '%' ";

        public static readonly string SearchDataInDB = @"
        SELECT DISTINCT ID, Plant, TypeOfContravention, CreatedBy, CreatedByName, CreatedDate, UpdatedBy, UpdatedByName, UpdatedDate, DelFlag
        FROM TypeOfContravention
        WHERE plant = @plant and TypeOfContravention LIKE '%' + @search + '%' ";

        public static readonly string InsertNewData = @"
        INSERT INTO TypeOfContravention (Plant,TypeOfContravention, CreatedBy, CreatedByName, CreatedDate) 
        VALUES (@plant,@TypeOfContravention, @userId, @userId, GETDATE())";

        public static readonly string UpdateData = @"
        UPDATE TypeOfContravention
        SET TypeOfContravention = @TypeOfContravention, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE()
        WHERE ID = @id";

        public static readonly string DeleteData = @"
        UPDATE TypeOfContravention
        SET DelFlag = 1, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE() 
        WHERE ID = @id";

        public static readonly string PermDeleteData = @"
        DELETE FROM TypeOfContravention WHERE ID = @id";

        public static readonly string RecoverData = @"
        UPDATE TypeOfContravention 
        SET DelFlag = 0, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE() 
        WHERE ID = @id";

        public static readonly string Import = @"
        UPDATE TypeOfContravention
        SET 
            Plant = B.Plant,
            TypeOfContravention = B.TypeOfContravention,
            UpdatedBy = UPPER(@userId),
            UpdatedByName = UPPER(@userId),
            UpdatedDate = GETDATE(),
            DelFlag = 0
        FROM TypeOfContravention A
        INNER JOIN ##temp B 
        ON (A.TypeOfContravention = B.TypeOfContravention AND A.Plant = B.Plant)

        INSERT INTO TypeOfContravention 
        (Plant,TypeOfContravention, CreatedBy, CreatedByName, CreatedDate, DelFlag) 
        SELECT 
            B.Plant,
            B.TypeOfContravention, 
            UPPER(@userId),
            UPPER(@userId),
            GETDATE(),
            0
        FROM ##temp B
        WHERE NOT EXISTS (
            SELECT A.TypeOfContravention
            FROM TypeOfContravention A 
            WHERE A.TypeOfContravention = B.TypeOfContravention AND A.Plant = B.Plant
        )";
    }
}
