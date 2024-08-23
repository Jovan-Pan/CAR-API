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

        public static readonly string GetTPRODUCT = @"
        select distinct A.Product from TPRODUCT A
        join TSMNProductPIC B on A.Plant = B.Plant and A.Product = B.Product
        where A.DelFlag = 0 and B.DelFlag = 0 and B.System = 'CAR' and A.Plant=@plant and B.Userid = @Userid
        order by A.Product asc
         ";

        public static readonly string GetMatGrp = @"
        select distinct Matgrp  from TMATERIAL
        where DelFlag = 0 and PlantStatus not in ('Z4','Z9') 
        and Plant = @plant and (Product = @product or @product is null)
        and Product in @productAuthList
        order by Matgrp asc
        ";

        public static readonly string GetMatType = @"
        SELECT distinct MaterialType,MESDesc FROM TMATERIALTYPE where DelFlag= 0 and Plant = @plant
        order by MaterialType asc
        ";

        public static readonly string GetMaterial = @"
        select distinct Material,MaterialDesc,Product,matgrp,MaterialType from 
        TMATERIAL 
        where DelFlag = 0 and PlantStatus not in ('Z4','Z9')
        and (Product = @product or @product is null) 
        and Product in @productAuthList
        and (matgrp = @matgroup or @matgroup is null)
        and (MaterialType = @mattype or @mattype is null)
        and (Material like '%'+ @searchTerm +'%' or Material like '%'+ MaterialDesc +'%') 
        order by Material asc
        ";

        public static readonly string GetNCCategory = @"
        select distinct concat(NCCode,'_',NCCategory) id, NCCode,NCCategory,NCReason 
        from TNCREASON where DelFlag = 0 
        order by NCCategory asc, NCReason asc
        ";

        public static readonly string GetSystemDeptVsUser = @"
        select distinct A.dept,B.deptName
        from TSYSTEMVSDEPT A 
        join TDEPT B on A.Plant = b.Plant and A.Dept = B.Dept 
        join Dept_Usr C on A.Plant = C.Plant and A.Dept = C.Dept and C.System = A.SysCode and C.isDeleted = 0
        where A.SysCode = 'CAR' and A.DelFlag = 0 and B.DelFlag = 0 and A.Plant = @plant  and C.UseID=@Userid 
        order by A.dept asc
        ";

        public static readonly string GetVendor = @"
        select distinct V.Vendor as vendorCode,V.Description as vendDesc 
        from tVendor_New V
        join tVendorPOrg P on V.POrg = P.POrg and P.Vendor = V.Vendor and P.DelFlag = 0
        where V.DelFlag = 0 and P.Plant = @plant
        order by V.Description asc
        ";
    }
}
