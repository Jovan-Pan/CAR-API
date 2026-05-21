using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Query
{
    public class IssueFeedbackReportQuery
    {
        public static readonly string GetTotalRecord = @" select count (*) from IssueFeedback
        where Plant = @plant ";

        public static readonly string getIssuerIdForNCR = @" 
        SELECT distinct UserList
        FROM 
        (
            SELECT TOP 1 
                IssueBy, 
                AcknowledgeBy, 
                PDAActionBy, 
                PDAAprovalBy, 
                ReceiveActionBy, 
                ReceiveAprovalBy, 
                PDAReviewBy, 
                ReviewBy
            FROM 
                IssueFeedback 
            WHERE 
                Plant = @plant 
                AND FormNo = @FormNo
        ) AS SourceTable
        UNPIVOT
        (
            UserList FOR UserType IN 
            (IssueBy, AcknowledgeBy, PDAActionBy, PDAAprovalBy, ReceiveActionBy, ReceiveAprovalBy, PDAReviewBy, ReviewBy)
        ) AS UnpivotedTable;
        ";

        public static readonly string getIssuerId = @"
        SELECT distinct UserList
        FROM 
        (
            SELECT 
                A.IssueBy, 
                A.AcknowledgeBy, 
                A.PDAActionBy, 
                A.PDAAprovalBy, 
                A.ReceiveActionBy, 
                A.ReceiveAprovalBy, 
                A.PDAReviewBy, 
                A.ReviewBy,
		        B.UseID
            FROM 
                IssueFeedback A inner join IssueFeedBackEmailRecipient B on A.FormNo = B.FormNo and B.UserLevel !='MailToCC'

            WHERE 
                   Plant = @plant 
                  AND A.FormNo = @FormNo AND B.FormNo = @FormNo 
        ) AS SourceTable
        UNPIVOT
        (
            UserList FOR UserType IN 
            (IssueBy, AcknowledgeBy, PDAActionBy, PDAAprovalBy, ReceiveActionBy, ReceiveAprovalBy, PDAReviewBy, ReviewBy,UseID)
        ) AS UnpivotedTable;"; 

        public static readonly string GetTotalRecordForEachStts = @"
        SELECT 
            COUNT(CASE WHEN (status = 'SUBMITED' or status = 'SUBMITED-REJECT' or status = 'RE-SUBMIT' or status = 'SUBMITED-APPEAL' or status = 'RE-SUBMIT-APPEAL' or status = 'DRAFT-SUBMIT') THEN 1 END) AS Submitted,
            COUNT(CASE WHEN (status in ('OPEN','OPEN-REJECT','OPEN-APPEAL') ) THEN 1 END) AS [Open],
            COUNT(CASE WHEN status in ('PDA-DECISION','PDA-DECISION-APPEAL') THEN 1 END) AS PdaDecision,
	        COUNT(CASE WHEN (status in ('ISSUED','ISSUED-REJECT','ISSUED-APPEAL') ) THEN 1 END) AS issued,
	        COUNT(CASE WHEN status = 'ACTION-ISSUED' THEN 1 END) AS ActIssued,
            COUNT(CASE WHEN status = 'ISSUED-REJECTED (WA)' THEN 1 END) AS ActIssuedRejecWA,
	        COUNT(CASE WHEN status = 'ANALYZE' THEN 1 END) AS Analize,
            COUNT(CASE WHEN status = 'ANALYZE-MANAGEMENT' THEN 1 END) AS AnalizeManagement,
	        COUNT(CASE WHEN status = 'REVIEW' THEN 1 END) AS review,
	        COUNT(CASE WHEN status = 'COMPLETE' THEN 1 END) AS Complete
        FROM IssueFeedback
        WHERE Plant = @plant and DetectionDate >= DATEADD(MONTH, -@defDataShow, GETDATE())   {0} 
        ";

        public static readonly string GetDefDataShow = @"
        select A.IDValue from tGlobal A 
        left join TPlantvsGlobal B on A.id = B.SettingID
        where A.id ='DefDataShow' And B.Plant = @plant and B.SysCode = 'CAR'";

        public static readonly string GetFormNumberListFilter = @" select distinct FormNo from IssueFeedback
        where Plant = @plant and Dept in @deptAuthList and (Product IN @productAuthList or 'ALL' IN @productAuthList)
        order by FormNo asc ";

        public static readonly string GetDeptListFilter = @" select distinct Dept from IssueFeedback
        where Plant = @plant and Dept in @deptAuthList and (Product IN @productAuthList or 'ALL' IN @productAuthList) order by Dept asc ";

        public static readonly string GetproductListFilter = @" select distinct Product from IssueFeedback
        where Plant = @plant and Dept in @deptAuthList and (Product IN @productAuthList or 'ALL' IN @productAuthList) order by product asc ";

        public static readonly string GetprocecessGrpCodeFilter = @" select distinct procecessGrpCode from IssueFeedback
        where Plant = @plant and Dept in @deptAuthList and (Product IN @productAuthList or 'ALL' IN @productAuthList) and isnull(procecessGrpCode,'') <> ''
        order by procecessGrpCode asc ";

        public static readonly string GetModelListFilter = @" select distinct Model from IssueFeedback
        where Plant = @plant and Dept in @deptAuthList and (Product IN @productAuthList or 'ALL' IN @productAuthList)
        order by model asc ";

        public static readonly string GetMatTypeListFilter = @" select distinct MaterialType from IssueFeedback
        where Plant = @plant and Dept in @deptAuthList and (Product IN @productAuthList or 'ALL' IN @productAuthList)
        order by MaterialType asc ";

        public static readonly string GetMaterialListFilter = @" select distinct MaterialCode from IssueFeedback
        where Plant = @plant and Dept in @deptAuthList and (Product IN @productAuthList or 'ALL' IN @productAuthList) AND MaterialCode IS NOT NULL
        order by MaterialCode asc ";

        public static readonly string GetVendorListFilter = @" select distinct VendorCode,VendorDesc as vendDesc from IssueFeedback
        where Plant = @plant and Dept in @deptAuthList and (Product IN @productAuthList or 'ALL' IN @productAuthList)
        order by VendorCode asc ";

        public static readonly string skiprow = @" OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY; ";

        public static readonly string GetMainData = @"
        SELECT  
            ifb.Plant,
            ifb.FormType,
            ifb.FormNo,
            ifb.DetectionDate,
            ifb.StatusOfFinding,
            ifb.Product,
            ifb.Model,
            ifb.MaterialType,
            ifb.MaterialCode,
            ifb.MaterialDesc,
            ifb.NcQty,
            ifb.SamplingCheck,
            ifb.NcRatio,
            ifb.Dept,
            ifb.VendorCode,
            ifb.VendorDesc,
            ifb.TttlQty,
            ifb.TttlQtyUOM,
            ifb.AffectedCavity,
            ifb.AffectedCavityNO,
            ifb.IssueType,
            ifb.NCCode,
            ifb.NCCategory,
            ifb.NCReason,
            ifb.NCDescription,
            ifb.Status,
            ifb.mainStatus,
            ifb.IssueBy,
            ifb.IssueByName,
            ifb.IssueDate,
            ifb.IssueByComment,
            ifb.IssueUpdatedBy,
            ifb.IssueUpdatedByName,
            ifb.IssueUpdatedDate,
            ifb.AcknowledgeBy,
            ifb.AcknowledgeByname,
            ifb.AcknowledgeByDate,
            ifb.AcknowledgeByComment,
            ifb.PDAActionBy,
            ifb.PDAActionByName,
            ifb.PDAActionDate,
            ifb.PDAActionImmAct,
            ifb.PDAActionComment,
            ifb.PDAActionUpdatedBy,
            ifb.PDAActionUpdatedByName,
            ifb.PDAActionUpdatedDate,
            ifb.PDAAprovalBy,
            ifb.PDAAprovalByName,
            ifb.PDAAprovalDate,
            ifb.PDAAprovalComment AS pdaAprovalComment,
            ifb.PDAVoidBy,
            ifb.PDAVoidByName,
            ifb.PDAVoidDate,
            ifb.PDAVoidComment,
            ifb.ImmActRecDetail,
            ifb.CostPC,
            ifb.Curency,
            ifb.ActionResult,
            ifb.ReceiveActionRootCause,
            ifb.RootCauseDetail,
            ifb.procecessGrpCode,
            ifb.ReceiveCorrectiveAct,
            ifb.EffectiveDate,
            ifb.ReceiveActionComment,
            ifb.ReceiveActionRejectReason,
            ifb.ReceiveActionBy,
            ifb.ReceiveActionByName,
            ifb.ReceiveActionDate,
            ifb.ReceiveActionUpdatedBy,
            ifb.ReceiveActionUpdatedByName,
            ifb.ReceiveActionUpdatedDate,
            ifb.ReceiveAprovalBy,
            ifb.ReceiveAprovalByName,
            ifb.ReceiveAprovalDate,
            ifb.receiveAprovalComment,
            ifb.Detectedby,
            ifb.[Plating Line No/Name] AS PlatingLineNoName,
            ifb.checkingMethod,
            ifb.PDAReviewBy,
            ifb.PDAReviewByName,
            ifb.PDAReviewDate,
            ifb.PDAReviewComment,
            ifb.MngReviewBy,
            ifb.MngReviewByName,
            ifb.MngReviewDate,
            ifb.ReviewBy,
            ifb.ReviewByName,
            ifb.ReviewDate,
            ifb.ReviewSubmitDate,
            ifb.ReviewComment,
            ifb.ReviewMethod,
            ifb.PossibleHazards,
            ifb.Typeofcontravention,
            ifb.RiskCategory,
            ifb.SourceofSupply,
            ifb.TeamSeaPlant,
            mp.PlantAbbreviation,
            ifb.FiveWhyOutput
        FROM IssueFeedback ifb
        LEFT JOIN MDMTPLANT mp ON ifb.TeamSeaPlant = mp.plant
        where ifb.Plant = @plant
        and (Dept IS NULL OR Dept IN @deptAuthList) and ((Product IN @productAuthList or 'ALL' IN @productAuthList) OR (Product IS NULL OR Product = ''))
        ";

        public static readonly string GetDataAttchment = @"
        select B.FormNo,B.ActionType,B.OriFileName,B.FileName,B.FileExt,B.FilePath from IssueFeedback A
         join IssueFeedbackAtchment B on A.FormNo = B.FormNo
         where A.Plant = @plant and A.FormNo IN @FormNo
        ";

        public static readonly string ProcessUpdate = @" 
        update IssueFeedback 
        set UpdateBy = @UserId
        ,UpdateByName =@UserName
        ,UpdatedDate = GETDATE()
        ,StatusOfFinding = @StatusOfFinding
        where FormNo = @FormNo and Plant = @UserPlant
        ";

        public static readonly string GetEmailRecipientsList = @"
        select B.FormNo,B.UseID,B.UseNam,B.UseEmail,B.UserLevel from IssueFeedback A
        join IssueFeedBackEmailRecipient B on A.FormNo = B.FormNo
        where A.Plant = @plant and A.FormNo IN @FormNoList";

        public static readonly string GetMailtocc = @"
        select distinct UseEmail from IssueFeedBackEmailRecipient where formno = @formno and UserLevel ='MailToCC'";

        public static readonly string GetMailToCCStatic = @"
        select distinct U.UseEmail
        from SystemvsUservsEmailSubscribeForm A
        join SystemvsUservsEmailSubscribeFormDetail B on A.ID = B.ID
        join Usr U on B.UserID = U.UseID
        join TSMNProductPIC P on P.Plant = A.Plant and P.Userid = b.UserID and P.DelFlag = 0
        join Dept_Usr DU on A.Plant = DU.Plant and DU.System = A.SystemCode and DU.UseID = B.UserID and DU.isDeleted = 0
        where a.IsDeleted = 0 and B.IsDeleted = 0
        and A.SystemCode = 'CAR' and A.Plant = @plant and A.[Group] = @group and DU.Dept = @dept AND EmailCCList = 1
        And B.UserID NOT IN(select UseID from uservsvendor)";

        public static readonly string GetAllIssuesForProcessing = @"
        DECLARE @StartDate DATE = '2026-04-27';

        SELECT * FROM (
            SELECT 
                C.MaterialDesc AS MaterialDescription,
                A.performedOn,
                B.*,
                ISNULL(TRY_CAST(G.IDValue AS INT), 7) AS ReminderDays,
                ROW_NUMBER() OVER (PARTITION BY A.formno ORDER BY A.performedOn DESC) AS rn
            FROM workflowhistory A
            JOIN IssueFeedback B ON A.formno = B.formno
            LEFT JOIN MDMTMATERIAL C ON B.MaterialCode = C.Material
            LEFT JOIN MDMTPLANTVSGLOBAL PVG 
                ON B.Plant = PVG.Plant 
                AND PVG.SettingID = 'DueAfter_Mail_Reminder' 
                AND PVG.SysCode = 'CAR'
            LEFT JOIN MDMTGLOBAL G ON PVG.SettingID = G.ID
            WHERE CAST(A.performedOn AS DATE) >= @StartDate  -- ⬅️ start dari hari ini
        ) AS subquery
        WHERE rn = 1 
          AND CAST(performedOn AS DATE) <= CAST(DATEADD(day, -ReminderDays, @StartDate) AS DATE)
          AND status IN ('ISSUED', 'ACTION-ISSUED');
        ";
    }
}
