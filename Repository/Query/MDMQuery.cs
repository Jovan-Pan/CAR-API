using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Query
{
    public class MDMQuery
    {
        public static readonly string GetUserVendorInfo = @" 
        select A.Vendor as vendorcode,B.Description as vendorname
        from USERVSVENDOR A 
        join tVendor_New B on A.Vendor = B.Vendor 
        join tVendorPOrg C on C.Vendor = C.Vendor and C.POrg = B.POrg and C.Plant = A.Plant and C.DelFlag = 0
        where A.Plant=@plant and UseID = @userid and A.DelFlag = 0 
        ";

        public static readonly string GetPlantList = @"
        Select distinct Plant 
        FROM TGROUP g
        INNER JOIN TUSER_AUTHORIZE ua on (g.GroupID = ua.GroupID)
        WHERE g.System = 'CAR' AND ua.UserId = @userId
        GROUP BY Plant";

        public static readonly string GetFormAuthorize = @"
        select distinct UA.ViewOnly isuserAuViewOnly ,GA.viewOnly, isnull(F.formdesc,GA.FormName) AS formdesc
        from TUSER_AUTHORIZE UA join TGROUPACCESS GA on UA.GroupID = GA.GroupID and UA.System = GA.System and UA.FormName = GA.FormName and isnull(GA.DelFlag,0) = 0 
        join TGROUP G on G.GroupID = GA.GroupID and G.SYSTEM = GA.System and ISNULL(G.DelFlag,0)=0 
        Join TFORM F on F.System = GA.System and ISNULL(F.DelFlag,0)=0 and F.FormName=GA.FormName 
        where upper(GA.System)= 'CAR' AND isnull(UA.DELFLAG,0) = 0
        and UPPER(UserId)=@userId and G.Plant = @plant and  upper(GA.FormName) IN @FormId  
        order by GA.ViewOnly asc";

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
        where DelFlag = 0 and isnull(PlantStatus,'') not in ('Z4','Z9') 
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
        where DelFlag = 0 and Plant = @plant  and isnull(PlantStatus,'') not in ('Z4','Z9')
        and (Product = @product or @product is null) 
        AND Product IN @productAuthList
        and (matgrp = @matgroup or @matgroup is null)
        and (MaterialType = @mattype or @mattype is null)
        and (Material like '%'+ @searchTerm +'%' or MaterialDesc like '%'+ @searchTerm +'%' or @searchTerm is null) 
        order by Material asc
        ";

        public static readonly string GetMaterialWoProdAut = @"
        select distinct Material,MaterialDesc,Product,matgrp,MaterialType from 
        TMATERIAL 
        where DelFlag = 0 and Plant = @plant  and isnull(PlantStatus,'') not in ('Z4','Z9')
        and (matgrp = @matgroup or @matgroup is null)
        and (MaterialType = @mattype or @mattype is null)
        and (Material like '%'+ @searchTerm +'%' or MaterialDesc like '%'+ @searchTerm +'%' or @searchTerm is null) 
        and Material in @MaterialList
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
        where A.SysCode = 'CAR' and A.DelFlag = 0 and B.DelFlag = 0 and A.Plant = @plant  and (C.UseID=@Userid or @Userid = 'CAR') 
        order by A.dept asc
        ";

        public static readonly string GetVendor = @"
        select distinct V.Vendor as vendorCode,V.Description as vendDesc 
        from tVendor_New V
        join tVendorPOrg P on V.POrg = P.POrg and P.Vendor = V.Vendor and P.DelFlag = 0
        where V.DelFlag = 0 and P.Plant = @plant
        order by V.Description asc
        ";

        public static readonly string getBasePathConfig = @"
        select plant,system,domain,userID,password,basePath
        from CommonADID
        where IsDeleted = 0 and Plant = @plant and system='CAR'
        ";

        public static readonly string getUserFormAuthorize = @"
        select distinct GA.FormName, isnull(F.formdesc,GA.FormName) AS formdesc,UA.ViewOnly IsUserAuViewOnly ,GA.ViewOnly
        from TUSER_AUTHORIZE UA join TGROUPACCESS GA on UA.GroupID = GA.GroupID and UA.System = GA.System and UA.FormName = GA.FormName and isnull(GA.DelFlag,0) = 0 
        join TGROUP G on G.GroupID = GA.GroupID and G.SYSTEM = GA.System and ISNULL(G.DelFlag,0)=0 
        Join TFORM F on F.System = GA.System and ISNULL(F.DelFlag,0)=0 and F.FormName=GA.FormName
        where GA.System= 'CAR' AND isnull(UA.DELFLAG,0) = 0 and G.Plant = @plant
        and UserId=@UserId  and GA.FormName = @FormName
        order by GA.ViewOnly asc
        ";

        public static readonly string getUserStatusAuthorize = @"
        select distinct replace(GA.FormName,'IS_','') as StatusAuthorize
        from TUSER_AUTHORIZE UA join TGROUPACCESS GA on UA.GroupID = GA.GroupID and UA.System = GA.System and UA.FormName = GA.FormName and isnull(GA.DelFlag,0) = 0 
        join TGROUP G on G.GroupID = GA.GroupID and G.SYSTEM = GA.System and ISNULL(G.DelFlag,0)=0 
        Join TFORM F on F.System = GA.System and ISNULL(F.DelFlag,0)=0 and F.FormName=GA.FormName
        where GA.System= 'CAR' AND isnull(UA.DELFLAG,0) = 0 and LEFT(GA.FormName,3) = 'IS_' and G.Plant = @plant  and UserId=@UserId
        ";

        public static readonly string GetCurrency = @"
        select distinct CurrencyCode,CurrencyDescription from Currency where isDeleted = 0
        ";

        public static readonly string getProcessGrp = @"
        select Process_Grp_code as procecessGrpCode, Process_Grp_Description as procecessGrpDesc from TPROCESGROUP_LIST
        where DelFlag = 0
        ";

        public static readonly string getReason = @"
        select distinct ReasonforRejection from TREASONFORMETREJECTION where SysCode = 'CAR' and DelFlag = 0 
        and Plant = @plant and reasontype = @reasontype
        order by ReasonforRejection asc
        ";

        public static readonly string GetTGlobalEmailSetting = @"
        select top 1 SysCode,Plant,EmailSubject,Emailbody,EmailFooter,Emaillink,ReplyMailid,HLevel,FromMailaddress,WStatus
        from TGlobalEmailSetting 
        where Delflag=0 and Plant = @plant and SysCode = 'CAR' and WStatus=@wStatus
        ";

        public static readonly string GetSystemvsUservsEmailSubscribeForm = @"
        select distinct A.Plant,A.SystemCode,A.Dept,B.UserID,A.[Group],NULL as CategoryName,U.UseEmail
        from SystemvsUservsEmailSubscribeForm A
        join SystemvsUservsEmailSubscribeFormDetail B on A.ID = B.ID
        join Usr U on B.UserID = U.UseID
        join TSMNProductPIC P on P.Plant = A.Plant and P.Userid = b.UserID and P.DelFlag = 0
        join Dept_Usr DU on A.Plant = DU.Plant and DU.System = A.SystemCode and DU.UseID = B.UserID and DU.isDeleted = 0
        where a.IsDeleted = 0 and B.IsDeleted = 0
        and A.SystemCode = 'CAR' and A.Plant = @plant and A.[Group] = @group and DU.Dept = @dept
        ";

        public static readonly string GetissuerEmail = @"
        select distinct UseEmail from Usr where UseID IN @UseID and DelFlag = 0
        ";
    }
}
