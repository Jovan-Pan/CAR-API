using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Query
{
    public class WorkFlowHistoryQuery
    {
        public static readonly string GetWorkFlowHistory = @"
        SELECT DISTINCT ID, plant, Role, status, formno, performedBy, performedon, decision, comment
        FROM WorkFlowHistory
        WHERE (formno = @formno OR @formno IS NULL) and plant =@plant";
        public static readonly string InsertNewData = @"
        INSERT INTO WorkFlowHistory (plant, role, status, formno, performedBy, performedon, decision, comment)
        values(@plant,@role,@status,@formno,@performedBy,GETDATE(),@decision,@comment)
        ";
    }
}
