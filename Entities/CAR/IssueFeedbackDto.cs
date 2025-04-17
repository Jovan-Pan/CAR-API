using Entities.MasterData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.CAR
{
    public class IssueFeedbackDto
    {
        public int Plant { get; set; }
        public string FormType { get; set; }
        public string FormNo { get; set; }
        public DateTime? DetectionDate { get; set; }
        public string StatusOfFinding { get; set; }
        public string Product { get; set; }
        public string Model { get; set; }
        public string MaterialType { get; set; }
        public string MESDesc { get; set; }
        public string MaterialCode { get; set; }
        public string MaterialDesc { get; set; }
        public decimal? NcQty { get; set; }
        public string SamplingCheck { get; set; }
        public decimal? NcRatio { get; set; }
        public string Dept { get; set; }
        public string deptName { get; set; }
        public string VendorCode { get; set; }
        public string VendorDesc { get; set; }
        public decimal TttlQty { get; set; }
        public string TttlQtyUOM { get; set; }
        public int AffectedCavity { get; set; }
        public string? AffectedCavityNO { get; set; }
        public string IssueType { get; set; }
        public string NCCode { get; set; }
        public string NCCategory { get; set; }
        public string NCReason { get; set; }
        public string NCDescription { get; set; }
        public string Status { get; set; }
        public string mainStatus { get; set; }
        
        public string IssueBy { get; set; }
        public string IssueByName { get; set; }
        public DateTime? IssueDate { get; set; }
        public string IssueByComment { get; set; }
        public string IssueUpdatedBy { get; set; }
        public string IssueUpdatedByName { get; set; }
        public DateTime? IssueUpdatedDate { get; set; }
        public string AcknowledgeBy { get; set; }
        public string AcknowledgeByname { get; set; }
        public DateTime? AcknowledgeByDate { get; set; }
        public string AcknowledgeByComment { get; set; }

        public string PDAActionBy { get; set; }
        public string PDAActionByName { get; set; }
        public DateTime? PDAActionDate { get; set; }
        public string PDAActionImmAct { get; set; }
        public string PDAActionComment { get; set; }
        public string PDAActionUpdatedBy { get; set; }
        public string PDAActionUpdatedByName { get; set; }
        public DateTime? PDAActionUpdatedDate { get; set; }
        public string PDAAprovalBy { get; set; }
        public string PDAAprovalByName { get; set; }
        public DateTime? PDAAprovalDate { get; set; }
        public string pdaAprovalComment { get; set; }
        
        
        public string ImmActRecDetail { get; set; }
        public decimal? CostPC { get; set; }
        public string Curency { get; set; }
        public string CurrencyDescription {  get; set; }
        public decimal? ActionResult { get; set; }
        public string ReceiveActionRootCause { get; set; }
        public string RootCauseDetail { get; set; }
        public string procecessGrpCode { get; set; }
        public string procecessGrpDesc { get; set; }
        public string ReceiveCorrectiveAct { get; set; }
        public DateTime? effectivedate { get; set; }
        public string ReceiveActionComment { get; set; }
        public string ReceiveActionRejectReason { get; set; }
        public string ReceiveActionBy { get; set; }
        public string ReceiveActionByName { get; set; }
        public DateTime? ReceiveActionDate { get; set; }
        public string ReceiveActionUpdatedBy { get; set; }
        public string ReceiveActionUpdatedByName { get; set; }
        public DateTime? ReceiveActionUpdatedDate { get; set; }
        public string ReceiveAprovalBy { get; set; }
        public string ReceiveAprovalByName { get; set; }
        public DateTime? ReceiveAprovalDate { get; set; }
        public string receiveAprovalComment { get; set; }
        
        public DateTime? ReviewDate { get; set; }
        public string PDAReviewComment { get; set; }
        public string PDAReviewBy { get; set; }
        public string PDAReviewByName { get; set; }
        public DateTime? PDAReviewDate { get; set; }


        public string ReviewComment { get; set; }
        public string ReviewMethod { get; set; }
        public string ReviewBy { get; set; }
        public string ReviewByName { get; set; }
        public DateTime? ReviewSubmitDate { get; set; }

        public string? PossibleHazards { get; set; }
        public string? Typeofcontravention { get; set; }
        public string? RiskCategory { get; set; }

        public string? Detectedby { get; set; }

        public IEnumerable<IssueFeedbackAtchmentDto>? dataAtch { get; set; }
       
        public IEnumerable<UsrDto>? EmailRecipientsList { get; set; }

    }
}
