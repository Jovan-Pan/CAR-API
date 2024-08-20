using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Query
{
    public class MDMQuery
    {
        public static readonly string GetPlantList = @"
        Select Plant 
        FROM TGROUP g
        INNER JOIN TUSER_AUTHORIZE ua on (g.GroupID = ua.GroupID)
        WHERE g.System = 'CAR' AND ua.UserId = @userId
        GROUP BY Plant";

        public static readonly string GetDataGlobalSetting = @"
        SELECT DISTINCT B.Plant, A.ID AS SettingId, split_data.data AS SettingValue
        FROM tGlobal A
        JOIN TPLANTVSGLOBAL B ON A.ID = B.SettingID AND B.DelFlag = 0 
        CROSS APPLY dbo.split(A.IDValue, ',') AS split_data
        WHERE B.SysCode = @System AND A.ID = @SettingID AND B.Plant = @Plant
        ";
    }
}
