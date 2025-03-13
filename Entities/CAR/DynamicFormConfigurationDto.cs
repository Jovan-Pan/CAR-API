using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.CAR
{
    public class DynamicFormConfigurationDto
    {
        public int? Id { get; set; }
        public string? plant { get; set; }
        public string? FormType { get; set; }
        public string? FieldNameForModel { get; set; }
        public string? FieldName { get; set; }
        public string? FieldType { get; set; }
        public int? FieldLength { get; set; }
        public bool? AllowNull { get; set; }
        public bool? Mandatory { get; set; }
        public string? FieldElement { get; set; }
        public string? OptionDataResource { get; set; }
        public string? DBResource { get; set; } 
        public string? Query { get; set; }
        public string? DataOption { get; set; }
        public int? Sequence { get; set; }
        public bool? DelFlag { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedByName { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? userid { get; set; }

        public List<Dictionary<string, object>>? ExecutedQueryResult { get; set; }

    }
    public class IssueFeedbackColumnInfo
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public string Length { get; set; }
        public bool AllowNull { get; set; }
    }

    public class UsingParameter
    {
        public string UsePlant { get; set; }
        public string UseUserId { get; set; }
    }

    public class TestingQueryParam
    {
        public string DBResource { get; set; }
        public string query { get; set; }
        public string userid { get; set; }
        public string plant { get; set; }
    }
}
