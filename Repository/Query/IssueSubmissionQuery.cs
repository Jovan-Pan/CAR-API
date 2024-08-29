using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Query
{
    public class IssueSubmissionQuery
    {
        public static readonly string GenerateNewFormNo = @"
        declare @lastformno nvarchar(20) =  (select MAX(REPLACE(FormNo,'F','')) from IssueFeedback where LEFT(FormNo,9) = ('F' + format(getdate(),'yyyyMMdd')) )
        declare @newformno nvarchar(20)
        if(@lastformno is null)
        begin
	        set @newformno = (select concat('F',format(getdate(),'yyyyMMdd'),'0001'))
        end
        else
        begin
	        declare @lastrunNo int = (select RIGHT(@lastformno,4))
	        declare @newrunNo int = (@lastrunNo + 1)
	        set @newformno = (select concat(LEFT(@lastformno,9),FORMAT(@newrunNo, '0000')))
        end
        select @newformno as FormNo
        ";

        public static readonly string InsertDataIssueFeedback = @"
        insert into IssueFeedback(
        Plant,FormNo,DetectionDate,Product,Model,MaterialType,MaterialCode,SamplingCheck,Dept,VendorCode,VendorDesc,TttlQty,TttlQtyUOM
        ,AffectedCavity,IssueType,NCCode,NCCategory,NCReason,NCDescription,Status,IssueBy,IssueByName,IssueDate,IssueByComment)
        values
        (
        @UserPlant,@FormNumber,@DetectionDate,@Product,@Model,@MaterialType,@MaterialCode,@SamplingCheck,@Dept,@VendorCode,@VendorDesc,@TttlQty,@TttlQtyUOM
        ,@AffectedCavity,@IssueType,@NCCode,@NCCategory,@NCReason,@NCDescription,'SUBMITED',@UserId,@UserName,GETDATE(),@Comment
        )
        ";

        public static readonly string InsertDataAtchIssuer = @"
        insert into IssueFeedbackAtchment(FormNo,ActionType,ActionCode,OriFileName,FileName,FileExt,FilePath)
        values
        (@FormNo,@ActionType,@ActionCode,@OriFileName,@FileName,@FileExt,@FilePath)
        ";

        public static readonly string issuerUpdateDataIssueFeedback = @"
        update IssueFeedback
        set DetectionDate = @DetectionDate,
        SamplingCheck = @SamplingCheck,
        TttlQty = @TttlQty,
        TttlQtyUOM = @TttlQtyUOM,
        AffectedCavity = @AffectedCavity,
        NCDescription = @NCDescription,
        IssueByComment = @Comment,
        IssueUpdatedBy = @UserId,
        IssueUpdatedByName = @UserName,
        IssueUpdatedDate = GETDATE()
        where FormNo = @FormNumber
        ";

        public static readonly string deleteDataAtchIssuer = @"
        delete from IssueFeedbackAtchment
        where FormNo = @FormNo and ActionCode = @ActionType and ActionCode = @ActionCode and FilePath=@FilePath
        ";

        public static readonly string issuerMngUpdate = @"
        update IssueFeedback
        set 
        Status = 'OPEN',
        IssueAcknowledgeByComment = @Comment,
        IssueAcknowledgeBy = @UserId,
        IssueAcknowledgeByname = @UserName,
        IssueAcknowledgeByDate = GETDATE()
        where FormNo = @FormNumber
        ";

        public static readonly string pdaDecision = @"
        update IssueFeedback
        set 
        Status = 'PDA-DECISION',
        PDAActionImmAct = @PDAImmediteAct,
        PDAActionComment = @Comment,
        PDAActionBy = @UserId,
        PDAActionByName = @UserName,
        PDAActionDate = GETDATE()
        where FormNo = @FormNumber
        ";

        public static readonly string pdaDecisionUpdate = @"
        update IssueFeedback
        set 
        PDAActionImmAct = @PDAImmediteAct,
        PDAActionComment = @Comment,
        PDAActionUpdatedBy = @UserId,
        PDAActionUpdatedByName = @UserName,
        PDAActionUpdatedDate = GETDATE()
        where FormNo = @FormNumber
        ";

        public static readonly string pdaApproval = @"
        update IssueFeedback
        set 
        Status = 'ISSUED',
        PDAAprovalBy = @UserId,
        PDAAprovalByName = @UserName,
        PDAAprovalDate = GETDATE()
        where FormNo = @FormNumber
        ";

        public static readonly string ReceiverAction = @"
        update IssueFeedback
        set 
        Status = 'ACTION-ISSUED',
        ImmActRecDetail = @ImmActRecDetail,
        CostPC = @CostPC,
        Curency = @Curency,
        ActionResult = @ActionResult,
        IssueReceiveActionRootCause = @rootcause,
        RootCauseDetail = @RootCauseDetail,
        procecessGrpCode = @procecessGrpCode,
        IssueReceiveActionCorrectiveAct = @correctiveAct,
        EffectiveDate = @EffectiveDate,
        IssueReceiveActionComment=@Comment,
        IssueReceiveActionBy = @UserId,
        IssueAcknowledgeByname = @UserName,
        IssueReceiveActionDate = GETDATE()
        where FormNo = @FormNumber
        ";
    }
}
