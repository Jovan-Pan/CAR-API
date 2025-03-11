using Entities.ParamRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.CAR
{
    public class DynamicFormParameterDTO : IssueSbmsAtchParam
    {
        public string? IssueStatus { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public int UserPlant { get; set; }
        public string? FormType { get; set; }
        public string? FormNumber { get; set; }
        public string? NCCategory { get; set; }
        public string? Comment { get; set; }
        public string? IssueType { get; set; }
        public string? Dept { get; set; }
        public string? VendorCode { get; set; }

        public string? rejectReason { get; set; }
        public string? PDAImmediteAct { get; set; }

        public string? ImmActRecDetail { get; set; }
        public decimal? CostPC { get; set; }
        public string? Curency { get; set; }
        public decimal? ActionResult { get; set; }
        public string? rootcause { get; set; }
        public string? RootCauseDetail { get; set; }
        public string? procecessGrpCode { get; set; }
        public string? correctiveAct { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? ReviewDate { get; set; }
        public string? ReviewMethod { get; set; }

        public string? mailWStatus { get; set; }
        public string? mailactionType { get; set; }
        public string? sendmailUserAction { get; set; }

        public string? MainStatus { get; set; }
        public List<DynamicParameter>? DynamicParameters { get; set; }
        public List<IssueFeedBackEmailRecipient>? IssueFeedBackEmailRecipients { get; set; }

        //IssueFeedBackEmailRecipient
        //public string? UseID { get; set; }
        //public string? UseNam { get; set; }
        //public string? UseEmail { get; set; }
        //public string? UserLevel { get; set; }
    }

    public class DynamicParameter
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
    }

    public class IssueFeedBackEmailRecipient
    {
        public string? UseID { get; set; }
        public string? UseNam { get; set; }
        public string? UseEmail { get; set; }
        public string? UserLevel { get; set; }
    }
}
