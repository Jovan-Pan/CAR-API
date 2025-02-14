using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Query
{
    public class TableMappingFieldNameQuery
    {
        public static readonly string GetTableMappingFieldName = @"
        SELECT DISTINCT ID, Plant, FieldName, UIDisplay, Language, CreatedBy, CreatedByName, CreatedDate, UpdatedBy, UpdatedByName, UpdatedDate, DelFlag
        FROM TableMappingFieldName
        WHERE 1=1";

        public static readonly string CheckExistingData = @"
        select ID, Plant, FieldName, UIDisplay, Language, CreatedBy, CreatedByName, CreatedDate, UpdatedBy, UpdatedByName, UpdatedDate, DelFlag
        FROM TableMappingFieldName 
        where Plant = @Plant and FieldName = @FieldName and [Language] = @Language";

        public static readonly string SearchDataADV = @"
        SELECT DISTINCT ID, Plant, FieldName, UIDisplay, Language, CreatedBy, CreatedByName, CreatedDate, UpdatedBy, UpdatedByName, UpdatedDate, DelFlag
        FROM TableMappingFieldName
        WHERE FieldName LIKE '%' + @SearchADVFN + '%' OR
        UIDisplay LIKE '%' + @SearchADVUID + '%' OR
        Language LIKE '%' + @SearchADVLANG + '%'";

        public static readonly string SearchDataInDB = @"
        SELECT DISTINCT ID, Plant, FieldName, UIDisplay, Language, CreatedBy, CreatedByName, CreatedDate, UpdatedBy, UpdatedByName, UpdatedDate, DelFlag
        FROM TableMappingFieldName
        WHERE FieldName LIKE '%' + @search + '%' OR
        UIDisplay LIKE '%' + @search + '%' OR
        Language LIKE '%' + @search + '%'";

        public static readonly string InsertNewData = @"
        INSERT INTO TableMappingFieldName (Plant,FieldName, UIDisplay, Language, CreatedBy, CreatedByName, CreatedDate) 
        VALUES (@plant, @FieldName, @UIDisplay, @Language, @userId, @userId, GETDATE())";

        public static readonly string UpdateData = @"
        UPDATE TableMappingFieldName
        SET UIDisplay = @UIDisplay, Language = @Language, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE()
        WHERE ID = @id";

        public static readonly string DeleteData = @"
        UPDATE TableMappingFieldName
        SET DelFlag = 1, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE() 
        WHERE ID = @id";

        public static readonly string PermDeleteData = @"
        DELETE FROM TableMappingFieldName WHERE ID = @id";

        public static readonly string RecoverData = @"
        UPDATE TableMappingFieldName 
        SET DelFlag = 0, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE() 
        WHERE ID = @id";

        public static readonly string Import = @"
        UPDATE TableMappingFieldName
        SET 
            Plant = B.Plant,
            FieldName = B.FieldName,
            UIDisplay = B.UIDisplay, 
            Language = B.Language,
            UpdatedBy = UPPER(@userId),
            UpdatedByName = UPPER(@userId),
            UpdatedDate = GETDATE(),
            DelFlag = 0
        FROM TableMappingFieldName A
        INNER JOIN ##temp B 
        ON (A.FieldName = B.FieldName)

        INSERT INTO TableMappingFieldName 
        (Plant, FieldName, UIDisplay, Language, CreatedBy, CreatedByName, CreatedDate, DelFlag) 
        SELECT 
            B.Plant,
            FieldName = B.FieldName,
            UIDisplay = B.UIDisplay, 
            Language = B.Language,
            UPPER(@userId),
            UPPER(@userId),
            GETDATE(),
            0
        FROM ##temp B
        WHERE NOT EXISTS (
            SELECT A.FieldName
            FROM TableMappingFieldName A 
            WHERE A.FieldName = B.FieldName
        )";
    }
}
