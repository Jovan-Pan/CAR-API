using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.MasterData
{
    public class TableMappingFieldNameDto
    {
        public int ID { get; set; }
        public int Plant { get; set; }
        public string FieldName { get; set; }
        public String UIDisplay { get; set; }
        public String Language { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedByName { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool DelFlag { get; set; }

        public string? FieldNameManipulate { get; set; }
    }

    public class CRUDTableMappingFieldNameDto
    {
        public int? ID { get; set; }
        public string? Plant { get; set; }
        public string? FieldName { get; set; }
        public String? UIDisplay { get; set; }
        public string? Language { get; set; }
        public string? userid { get; set; }
    }
}
