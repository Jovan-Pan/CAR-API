using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.CAR
{
    public class DynamicFlowConfigurationDto
    {
        public int ID { get; set; }
        public string Plant { get; set; }
        public string FormType { get; set; }
        public string Flow { get; set; }
        public bool DelFlag { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedByName { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
    public class CRUDDynamicFlowConfigurationDto
    {
        public int? ID { get; set; }
        public string? Plant { get; set; }
        public string? FormType { get; set; }
        public string? Flow { get; set; }
        public string? Userid { get; set; }

    }
}
