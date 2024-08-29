using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ParamRequest
{
    public class IssueSubmissionParameters: IssueSbmsAtchParam
    {
        public string? IssueStatus { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int UserPlant { get; set; }
        public string? FormNumber { get; set; }
        public DateTime? DetectionDate { get; set; }
        public string? Product { get; set; }
        public string? Model { get; set; }
        public string? MaterialType { get; set; }
        public string? MaterialCode { get; set; }
        public int? SamplingCheck { get; set; }
        public string? Dept { get; set; }
        public string? VendorCode { get; set; }
        public string? VendorDesc { get; set; }
        public decimal? TttlQty { get; set; }
        public string? TttlQtyUOM { get; set; }
        public int? AffectedCavity { get; set; }
        public string? IssueType { get; set; }
        public string? NCCode { get; set; }
        public string? NCCategory { get; set; }
        public string? NCReason { get; set; }
        public string? NCDescription { get; set; }
        public string? Comment { get; set; }
        public string? PDAImmediteAct { get; set; }

        public string? ImmActRecDetail { get; set; }
        public decimal? CostPC { get; set; }
        public string? Curency { get; set; }
        public string? ActionResult { get; set; }
        public string? rootcause { get; set; }
        public string? RootCauseDetail { get; set; }
        public string? procecessGrpCode { get; set; }
        public string? correctiveAct { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }
}
