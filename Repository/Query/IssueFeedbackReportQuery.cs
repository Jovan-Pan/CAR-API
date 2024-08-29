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

        public static readonly string skiprow = @" OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY; ";

        public static readonly string GetMainData = @"
        select  
        Plant,FormNo,DetectionDate,Product,Model,MaterialType,MaterialCode,SamplingCheck,Dept,VendorCode
        ,VendorDesc,TttlQty,TttlQtyUOM,AffectedCavity,IssueType,NCCode,NCCategory,NCReason,NCDescription,Status
        ,IssueBy,IssueByName,IssueDate,IssueByComment
        ,IssueUpdatedBy,IssueUpdatedByName,IssueUpdatedDate
        ,IssueAcknowledgeBy,IssueAcknowledgeByname,IssueAcknowledgeByDate,IssueAcknowledgeByComment
        ,PDAActionBy,PDAActionByName,PDAActionDate,PDAActionImmAct,PDAActionComment
        ,PDAActionUpdatedBy,PDAActionUpdatedByName,PDAActionUpdatedDate
        ,PDAAprovalBy,PDAAprovalByName,PDAAprovalDate
        ,ImmActRecDetail,CostPC,Curency,ActionResult
        ,IssueReceiveActionRootCause,RootCauseDetail,procecessGrpCode
        ,IssueReceiveActionCorrectiveAct,EffectiveDate,IssueReceiveActionComment
        ,IssueReceiveActionBy,IssueReceiveActionByName,IssueReceiveActionDate
        ,IssueReceiveActionUpdatedBy,IssueReceiveActionUpdatedByName,IssueReceiveActionUpdatedDate
        ,IssueReceiveAprovalBy,IssueReceiveAprovalByName,IssueReceiveAprovalDate
        ,IssueReviewBy,IssueReviewByName,IssueReviewDate,IssueReviewComment,IssueReviewMethod
        ,IssueReviewUpdatedBy,IssueReviewUpdatedByName,IssueReviewUpdatedDate
        ,IssueReviewAprovalBy,IssueReviewAprovalByName,IssueReviewAprovalDate,IssueReviewAprovalComment
        from IssueFeedback
        where Plant = @plant
        ";

        public static readonly string GetDataAttchment = @"
        select B.FormNo,B.ActionType,B.ActionCode,B.OriFileName,B.FileName,B.FileExt,B.FilePath from IssueFeedback A
         join IssueFeedbackAtchment B on A.FormNo = B.FormNo
         where A.Plant = @plant and A.FormNo IN @FormNo
        ";
    }
}
