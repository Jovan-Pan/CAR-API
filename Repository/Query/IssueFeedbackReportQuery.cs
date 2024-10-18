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

        public static readonly string getIssuerId = @" 
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

        public static readonly string GetTotalRecordForEachStts = @" 
        SELECT 
            COUNT(CASE WHEN (status = 'SUBMITED' or status = 'SUBMITED-REJECT' or status = 'RE-SUBMIT') THEN 1 END) AS Submitted,
            COUNT(CASE WHEN (status = 'OPEN' or status = 'OPEN-REJECT') THEN 1 END) AS [Open],
            COUNT(CASE WHEN status = 'PDA-DECISION' THEN 1 END) AS PdaDecision,
	        COUNT(CASE WHEN (status = 'ISSUED' or status = 'ISSUED-REJECT') THEN 1 END) AS issued,
	        COUNT(CASE WHEN status = 'ACTION-ISSUED' THEN 1 END) AS ActIssued,
            COUNT(CASE WHEN status = 'ISSUED-REJECTED (WA)' THEN 1 END) AS ActIssuedRejecWA,
	        COUNT(CASE WHEN status = 'ANALYZE' THEN 1 END) AS Analize,
	        COUNT(CASE WHEN status = 'REVIEW' THEN 1 END) AS review,
	        COUNT(CASE WHEN status = 'COMPLETE' THEN 1 END) AS Complete
        FROM IssueFeedback
        WHERE Plant = @plant and DetectionDate >= DATEADD(MONTH, -@defDataShow, GETDATE())   {0} 
        ";

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
        where Plant = @plant and Dept in @deptAuthList and (Product IN @productAuthList or 'ALL' IN @productAuthList)
        order by MaterialCode asc ";

        public static readonly string GetVendorListFilter = @" select distinct VendorCode,VendorDesc as vendDesc from IssueFeedback
        where Plant = @plant and Dept in @deptAuthList and (Product IN @productAuthList or 'ALL' IN @productAuthList)
        order by VendorCode asc ";

        public static readonly string skiprow = @" OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY; ";

        public static readonly string GetMainData = @"
        select  
        Plant,FormType,FormNo,DetectionDate,Product,Model,MaterialType,MaterialCode,NcQty,SamplingCheck,NcRatio,Dept,VendorCode
        ,VendorDesc,TttlQty,TttlQtyUOM,AffectedCavity,IssueType,NCCode,NCCategory,NCReason,NCDescription,Status
        ,IssueBy,IssueByName,IssueDate,IssueByComment
        ,IssueUpdatedBy,IssueUpdatedByName,IssueUpdatedDate
        ,AcknowledgeBy,AcknowledgeByname,AcknowledgeByDate,AcknowledgeByComment
        ,PDAActionBy,PDAActionByName,PDAActionDate,PDAActionImmAct,PDAActionComment
        ,PDAActionUpdatedBy,PDAActionUpdatedByName,PDAActionUpdatedDate
        ,PDAAprovalBy,PDAAprovalByName,PDAAprovalDate,PDAAprovalComment as pdaAprovalComment
        ,ImmActRecDetail,CostPC,Curency,ActionResult
        ,ReceiveActionRootCause,RootCauseDetail,procecessGrpCode
        ,ReceiveCorrectiveAct,EffectiveDate,ReceiveActionComment,ReceiveActionRejectReason
        ,ReceiveActionBy,ReceiveActionByName,ReceiveActionDate
        ,ReceiveActionUpdatedBy,ReceiveActionUpdatedByName,ReceiveActionUpdatedDate
        ,ReceiveAprovalBy,ReceiveAprovalByName,ReceiveAprovalDate,receiveAprovalComment

        ,PDAReviewBy,PDAReviewByName,PDAReviewDate,PDAReviewComment

        ,ReviewBy,ReviewByName,ReviewDate,ReviewSubmitDate,ReviewComment,ReviewMethod
        from IssueFeedback
        where Plant = @plant
        and Dept IN @deptAuthList and (Product IN @productAuthList or 'ALL' IN @productAuthList)
        ";

        public static readonly string GetDataAttchment = @"
        select B.FormNo,B.ActionType,B.OriFileName,B.FileName,B.FileExt,B.FilePath from IssueFeedback A
         join IssueFeedbackAtchment B on A.FormNo = B.FormNo
         where A.Plant = @plant and A.FormNo IN @FormNo
        ";
    }
}
