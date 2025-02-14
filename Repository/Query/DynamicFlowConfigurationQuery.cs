using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Query
{
    public class DynamicFlowConfigurationQuery
    {
        public static readonly string GetDynamicFlowConfiguration = @"
        SELECT DISTINCT ID, Plant, FormType, Flow, CreatedBy, CreatedByName, CreatedDate, UpdatedBy, UpdatedByName, UpdatedDate, DelFlag
        FROM DynamicFlowConfiguration
        WHERE 1=1";

        public static readonly string SearchADV = @"
        SELECT DISTINCT ID, Plant, FormType, Flow, CreatedBy, CreatedByName, CreatedDate, UpdatedBy, UpdatedByName, UpdatedDate, DelFlag
        FROM DynamicFlowConfiguration
        WHERE FormType LIKE '%' + @SearchFTADV + '%' OR Flow LIKE '%' + @SearchADV + '%'";

        public static readonly string SearchInDB = @"
        SELECT DISTINCT ID, Plant, FormType, Flow, CreatedBy, CreatedByName, CreatedDate, UpdatedBy, UpdatedByName, UpdatedDate, DelFlag
        FROM DynamicFlowConfiguration
        WHERE Flow LIKE '%' + @search + '%'";

        public static readonly string InsertNewData = @"
        INSERT INTO DynamicFlowConfiguration (FormType,Plant,Flow, CreatedBy, CreatedByName, CreatedDate) 
        VALUES (@FormType,@plant,@Flow, @userId, @userId, GETDATE())";

        public static readonly string UpdateData = @"
        UPDATE DynamicFlowConfiguration
        SET  FormType = @FormType, Flow = @Flow, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE()
        WHERE ID = @id";

        public static readonly string DeleteData = @"
        UPDATE DynamicFlowConfiguration
        SET DelFlag = 1, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE() 
        WHERE ID = @id";

        public static readonly string PermDeleteData = @"
        DELETE FROM DynamicFlowConfiguration WHERE ID = @id";

        public static readonly string RecoverData = @"
        UPDATE DynamicFlowConfiguration 
        SET DelFlag = 0, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE() 
        WHERE ID = @id";

        public static readonly string Import = @"
        UPDATE DynamicFlowConfiguration
        SET 
            Plant = B.Plant,
            FormType = B.FormType,
            Flow = B.Flow,
            UpdatedBy = UPPER(@userId),
            UpdatedByName = UPPER(@userId),
            UpdatedDate = GETDATE(),
            DelFlag = 0
        FROM DynamicFlowConfiguration A
        INNER JOIN ##temp B 
        ON (A.FormType = B.FormType)

        INSERT INTO DynamicFlowConfiguration 
        (Plant ,FormType, Flow, CreatedBy, CreatedByName, CreatedDate, DelFlag) 
        SELECT
            B.Plant,
            B.FormType,
            B.Flow, 
            UPPER(@userId),
            UPPER(@userId),
            GETDATE(),
            0
        FROM ##temp B
        WHERE NOT EXISTS (
            SELECT A.Flow
            FROM DynamicFlowConfiguration A 
            WHERE A.FormType = B.FormType
        )";
    }
}
