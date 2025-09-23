using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Query
{
    public class ProcessQuery
    {
        public static readonly string GETProcess = @"
        SELECT DISTINCT ID, Plant, Process, CreatedBy, CreatedByName, CreatedDate, UpdatedBy, UpdatedByName, UpdatedDate, DelFlag
        FROM Process
        WHERE Plant =@Plant";

        public static readonly string SearchProcess = @"
        SELECT DISTINCT ID, Plant, Process, CreatedBy, CreatedByName, CreatedDate, UpdatedBy, UpdatedByName, UpdatedDate, DelFlag
        FROM Process
        WHERE Plant =@Plant AND Process LIKE '%' + @SearchADV + '%'";

        public static readonly string SearchProcessInDB = @"
        SELECT DISTINCT ID, Plant, Process, CreatedBy, CreatedByName, CreatedDate, UpdatedBy, UpdatedByName, UpdatedDate, DelFlag
        FROM Process
        WHERE Plant =@Plant AND Process LIKE '%' + @search + '%'";

        public static readonly string InsertNewData = @"
        INSERT INTO Process (Plant,Process, CreatedBy, CreatedByName, CreatedDate) 
        VALUES (@plant,@Process, @userId, @userId, GETDATE())";

        public static readonly string UpdateData = @"
        UPDATE Process
        SET Process = @Process, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE()
        WHERE ID = @id";

        public static readonly string DeleteData = @"
        UPDATE Process
        SET DelFlag = 1, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE() 
        WHERE ID = @id";

        public static readonly string PermDeleteData = @"
        DELETE FROM Process WHERE ID = @id";

        public static readonly string RecoverData = @"
        UPDATE Process 
        SET DelFlag = 0, UpdatedBy = @userId, UpdatedByName = @userId, UpdatedDate = GETDATE() 
        WHERE ID = @id";

        public static readonly string Import = @"
        UPDATE Process
        SET 
            Plant = B.Plant,
            Process = B.Process,
            UpdatedBy = UPPER(@userId),
            UpdatedByName = UPPER(@userId),
            UpdatedDate = GETDATE(),
            DelFlag = 0
        FROM Process A
        INNER JOIN ##temp B 
        ON (A.Process = B.Process AND A.Plant = B.Plant)

        INSERT INTO Process 
        (Plant,Process, CreatedBy, CreatedByName, CreatedDate, DelFlag) 
        SELECT
            B.Plant,
            B.Process, 
            UPPER(@userId),
            UPPER(@userId),
            GETDATE(),
            0
        FROM ##temp B
        WHERE NOT EXISTS (
            SELECT A.Process
            FROM Process A 
            WHERE A.Process = B.Process AND A.Plant = B.Plant
        )";
    }
}
