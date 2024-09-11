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
        --declare @FormType nvarchar(20) = 'CAR', @plant int = 2100
         declare @lastformno nvarchar(20) =  (select MAX(REPLACE(FormNo,@FormType,'')) 
								         from IssueFeedback 
								         where Plant=@plant and FormType=@FormType 
								         and left(replace(FormNo,(@FormType+CONVERT(nvarchar(max),@plant)),''),8) = (format(getdate(),'yyyyMMdd')) )
        set @lastformno = (SELECT LEFT(@lastformno, (LEN(@lastformno) - CHARINDEX('-', REVERSE(@lastformno)) + 1) - 1))
        declare @newformno nvarchar(20)
        if(@lastformno is null)
        begin
            set @newformno = (select concat(@FormType,@plant,format(getdate(),'yyyyMMdd'),'0001'))
        end
        else
        begin
            declare @lastrunNo int = (select RIGHT(@lastformno,4))
            declare @newrunNo int = (@lastrunNo + 1)
            set @newformno = (select concat(@FormType,@plant,format(getdate(),'yyyyMMdd'),FORMAT(@newrunNo, '0000')))
        end
        select @newformno as FormNo


        ";

        public static readonly string GenerateNewFormNoWithVer = @"
        DECLARE @OriFormNo NVARCHAR(30) = (SELECT LEFT(@FormNumber, (LEN(@FormNumber) - CHARINDEX('-', REVERSE(@FormNumber)) + 1) - 1))
        DECLARE @LastFormNoVers INT = (
        select top 1 SUBSTRING(FormNo,LEN(FormNo) - CHARINDEX('-', REVERSE(FormNo)) + 2, LEN(FormNo)) as DocVers
        from IssueFeedback where (SELECT LEFT(FormNo, (LEN(FormNo) - CHARINDEX('-', REVERSE(FormNo)) + 1) - 1)) = @OriFormNo
        order by  SUBSTRING(FormNo,LEN(FormNo) - CHARINDEX('-', REVERSE(FormNo)) + 2, LEN(FormNo)) desc)
        select concat(@OriFormNo,'-',FORMAT((@LastFormNoVers+1), '00')) as FormNo
        ";

        public static readonly string InsertDataIssueFeedback = @"
        insert into IssueFeedback(
        Plant,FormType,FormNo,DetectionDate,Product,Model,MaterialType,MaterialCode,SamplingCheck,Dept,VendorCode,VendorDesc,TttlQty,TttlQtyUOM
        ,AffectedCavity,IssueType,NCCode,NCCategory,NCReason,NCDescription,Status,IssueBy,IssueByName,IssueDate,IssueByComment)
        values
        (
        @UserPlant,@FormType,@FormNumber,@DetectionDate,@Product,@Model,@MaterialType,@MaterialCode,@SamplingCheck,@Dept,@VendorCode,@VendorDesc,@TttlQty,@TttlQtyUOM
        ,@AffectedCavity,@IssueType,@NCCode,@NCCategory,@NCReason,@NCDescription,'SUBMITED',@UserId,@UserName,GETDATE(),@Comment
        )
        ";

        public static readonly string InsertDataAtchIssuer = @"
        insert into IssueFeedbackAtchment(FormNo,ActionType,OriFileName,FileName,FileExt,FilePath)
        values
        (@FormNo,@ActionType,@OriFileName,@FileName,@FileExt,@FilePath)
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
        where FormNo = @FormNo and ActionType = @ActionType and FilePath=@FilePath
        ";

        public static readonly string issuerMngUpdate = @"
        update IssueFeedback
        set 
        Status = 'OPEN',
        AcknowledgeByComment = @Comment,
        AcknowledgeBy = @UserId,
        AcknowledgeByname = @UserName,
        AcknowledgeByDate = GETDATE()
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
        ReceiveActionRejectReason = NULL,
        ImmActRecDetail = @ImmActRecDetail,
        CostPC = @CostPC,
        Curency = @Curency,
        ActionResult = @ActionResult,
        ReceiveActionRootCause = @rootcause,
        RootCauseDetail = @RootCauseDetail,
        procecessGrpCode = @procecessGrpCode,
        ReceiveCorrectiveAct = @correctiveAct,
        EffectiveDate = @EffectiveDate,
        ReceiveActionComment=@Comment,
        ReceiveActionBy = @UserId,
        ReceiveActionByName = @UserName,
        ReceiveActionDate = GETDATE()
        where FormNo = @FormNumber
        ";

        public static readonly string ReceiverActionUpdate = @"
        update IssueFeedback
        set 
        Status = 'ACTION-ISSUED',
        ReceiveActionRejectReason = NULL,
        ImmActRecDetail = @ImmActRecDetail,
        CostPC = @CostPC,
        Curency = @Curency,
        ActionResult = @ActionResult,
        ReceiveActionRootCause = @rootcause,
        RootCauseDetail = @RootCauseDetail,
        procecessGrpCode = @procecessGrpCode,
        ReceiveCorrectiveAct = @correctiveAct,
        EffectiveDate = @EffectiveDate,
        ReceiveActionComment=@Comment,
        ReceiveActionUpdatedBy = @UserId,
        ReceiveActionUpdatedByName = @UserName,
        ReceiveActionUpdatedDate = GETDATE()
        where FormNo = @FormNumber
        ";

        public static readonly string ReceiverIssueReject = @"
        update IssueFeedback
        set 
        Status = 'ISSUED-REJECTED (WA)',
        ReceiveActionRejectReason = @rejectReason,
        ReceiveActionBy = @UserId,
        ReceiveActionByName = @UserName,
        ReceiveActionDate = GETDATE()
        where FormNo = @FormNumber
        ";

        public static readonly string ReceiverApproval = @"
        update IssueFeedback
        set 
        Status = 'ANALYZE',
        ReceiveAprovalBy = @UserId,
        ReceiveAprovalByName = @UserName,
        ReceiveAprovalDate = GETDATE()
        where FormNo = @FormNumber
        ";

        public static readonly string ReceiverApprovalToReject = @"
        update IssueFeedback
        set 
        Status = 'ISSUED-REJECTED',
        ReceiveAprovalBy = @UserId,
        ReceiveAprovalByName = @UserName,
        ReceiveAprovalDate = GETDATE()
        where FormNo = @FormNumber
        ";

        public static readonly string PDAReviewerVoid = @"
        update IssueFeedback
        set 
        Status = 'VOID',
        isPDAReviewResultAprov = 0,
        PDAReviewBy = @UserId,
        PDAReviewByName = @UserName,
        PDAReviewDate = GETDATE(),
        PDAReviewComment = @rejectReason
        where FormNo = @FormNumber
        ";

        public static readonly string PDAReviewerReject = @"
        update IssueFeedback
        set 
        Status = 'REJECT',
        isPDAReviewResultAprov = 0,
        PDAReviewBy = @UserId,
        PDAReviewByName = @UserName,
        PDAReviewDate = GETDATE(),
        PDAReviewComment = @rejectReason
        where FormNo = @FormNumber
        ";

        public static readonly string CreateNewIssueFeedBcakWithVers = @"
        insert into IssueFeedback(
        Plant,FormType,FormNo,DetectionDate,Product,Model,MaterialType,MaterialCode,SamplingCheck,Dept,VendorCode,VendorDesc,TttlQty,TttlQtyUOM
        ,AffectedCavity,IssueType,NCCode,NCCategory,NCReason,NCDescription,Status,IssueBy,IssueByName,IssueDate,IssueByComment,AcknowledgeBy,AcknowledgeByname,AcknowledgeByDate,AcknowledgeByComment)
        select Plant,FormType,@NewFormNumber,DetectionDate,Product,Model,MaterialType,MaterialCode,SamplingCheck,Dept,VendorCode,VendorDesc,TttlQty,TttlQtyUOM
        ,AffectedCavity,IssueType,NCCode,NCCategory,NCReason,NCDescription,'OPEN',@UserId,@UserName,GETDATE(),IssueByComment,AcknowledgeBy,AcknowledgeByname,AcknowledgeByDate,AcknowledgeByComment
        from IssueFeedback where FormNo = @OldFormNumber
        ";

        public static readonly string PDAReviewerAprove = @"
        update IssueFeedback
        set 
        Status = 'REVIEW',
        isPDAReviewResultAprov = 1,
        PDAReviewBy = @UserId,
        PDAReviewByName = @UserName,
        PDAReviewDate = GETDATE(),
        PDAReviewComment = @Comment
        where FormNo = @FormNumber
        ";

        public static readonly string ReviewerSubmit = @"
        update IssueFeedback
        set 
        Status = 'COMPLETE',
        ReviewComment = @Comment,
        ReviewMethod = @ReviewMethod,
        isReviewResultAprov = 1,
        ReviewBy = @UserId,
        ReviewByName = @UserName,
        ReviewSubmitDate = GETDATE()
        where FormNo = @FormNumber
        ";

        public static readonly string ReviewerReject = @"
        update IssueFeedback
        set 
        Status = 'NOT EFFECTIVE',
        isReviewResultAprov = 0,
        ReviewBy = @UserId,
        ReviewByName = @UserName,
        ReviewSubmitDate = GETDATE(),
        ReviewComment = @rejectReason
        where FormNo = @FormNumber
        ";

        public static readonly string cekAvailableCompletePastIssue = @"
        select FormNo from IssueFeedback 
        where Plant = @plant and MaterialCode = @material and Dept=@dept 
        and (VendorCode=@vendor or @vendor is null)
        and procecessGrpCode = @processgroup and DATEDIFF(MONTH, DetectionDate, GETDATE()) >= @SetFormTypeStatusRange
        ";
    }
}
