using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.CAR
{
    public class ImmidateActionDto
    {
        public int id { get; set; }
        public string ImmidateName { get; set; }
        public int Plant { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool DelFlag { get; set; }
    }
    public class GETImmidateAction
    {
        public string? search { get; set; }
        public string? SearchADV { get; set; }
        public bool delflag { get; set; }
        public string? plant { get; set; }
    }
}
