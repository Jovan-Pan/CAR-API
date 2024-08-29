using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.CAR
{
    public class IssueFeedbackDto
    {
        public int Plant { get; set; }
        public string FormNo { get; set; }
        public DateTime DetectionDate { get; set; }
        public string Product { get; set; }
        public string Model { get; set; }
        public string MaterialType { get; set; }
        public string MESDesc { get; set; }
        public string MaterialCode { get; set; }
        public string MaterialDesc { get; set; }
        
        public string SamplingCheck { get; set; }
        public string Dept { get; set; }
        public string deptName { get; set; }
        public string VendorCode { get; set; }
        public string VendorDesc { get; set; }
        public decimal TttlQty { get; set; }
        public string TttlQtyUOM { get; set; }
        public int AffectedCavity { get; set; }
        public string IssueType { get; set; }
        public string NCCode { get; set; }
        public string NCCategory { get; set; }
        public string NCReason { get; set; }
        public string NCDescription { get; set; }
        public string Status { get; set; }
        public string IssueBy { get; set; }
        public string IssueByName { get; set; }
        public DateTime IssueDate { get; set; }
        public string IssueByComment { get; set; }
        public string IssueUpdatedBy { get; set; }
        public string IssueUpdatedByName { get; set; }
        public DateTime IssueUpdatedDate { get; set; }
        public string IssueAcknowledgeBy { get; set; }
        public string IssueAcknowledgeByname { get; set; }
        public DateTime IssueAcknowledgeByDate { get; set; }
        public string IssueAcknowledgeByComment { get; set; }
        public string PDAActionBy { get; set; }
        public string PDAActionByName { get; set; }
        public DateTime PDAActionDate { get; set; }
        public string PDAActionImmAct { get; set; }
        public string PDAActionComment { get; set; }
        public string PDAActionUpdatedBy { get; set; }
        public string PDAActionUpdatedByName { get; set; }
        public DateTime PDAActionUpdatedDate { get; set; }
        public string PDAAprovalBy { get; set; }
        public string PDAAprovalByName { get; set; }
        public DateTime PDAAprovalDate { get; set; }

        public string ImmActRecDetail { get; set; }
        public decimal? CostPC { get; set; }
        public string Curency { get; set; }
        public string CurrencyDescription {  get; set; }
        public string ActionResult { get; set; }
        public string IssueReceiveActionRootCause { get; set; }
        public string RootCauseDetail { get; set; }
        public string procecessGrpCode { get; set; }
        public string procecessGrpDesc { get; set; }
        public string IssueReceiveActionCorrectiveAct { get; set; }
        public DateTime? effectivedate { get; set; }
        public string IssueReceiveActionComment { get; set; }
        public string IssueReceiveActionBy { get; set; }
        public string IssueReceiveActionByName { get; set; }
        public DateTime IssueReceiveActionDate { get; set; }
        public string IssueReceiveActionUpdatedBy { get; set; }
        public string IssueReceiveActionUpdatedByName { get; set; }
        public DateTime IssueReceiveActionUpdatedDate { get; set; }
        public string IssueReceiveAprovalBy { get; set; }
        public string IssueReceiveAprovalByName { get; set; }
        public DateTime IssueReceiveAprovalDate { get; set; }
        public string IssueReviewBy { get; set; }
        public string IssueReviewByName { get; set; }
        public DateTime IssueReviewDate { get; set; }
        public string IssueReviewComment { get; set; }
        public string IssueReviewMethod { get; set; }
        public string IssueReviewUpdatedBy { get; set; }
        public string IssueReviewUpdatedByName { get; set; }
        public DateTime IssueReviewUpdatedDate { get; set; }
        public string IssueReviewAprovalBy { get; set; }
        public string IssueReviewAprovalByName { get; set; }
        public DateTime IssueReviewAprovalDate { get; set; }
        public string IssueReviewAprovalComment { get; set; }

    }
}
