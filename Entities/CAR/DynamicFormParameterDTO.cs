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
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int UserPlant { get; set; }
        public string? FormType { get; set; }
        public string? FormNumber { get; set; }
        public string? NCCategory { get; set; }
        public string? Comment { get; set; }
        public string? IssueType { get; set; }
        public List<DynamicParameter>? DynamicParameters { get; set; }
    }

    public class DynamicParameter
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
    }
}
