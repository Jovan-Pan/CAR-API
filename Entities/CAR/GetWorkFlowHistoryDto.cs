using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.CAR
{
    public class GetWorkFlowHistoryDto
    {
        public int ID { get; set; }
        public string Plant { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public string FormNo { get; set; }
        public string PerformedBy { get; set; }
        public DateTime PerformedOn { get; set; }
        public string Decision { get; set; }
        public string? Comment { get; set; }
    }
    public class CRUDWorkFlowHistoryDto
    {
        public int? id { get; set; }
        public string? Plant { get; set; }
        public string? Typeofcontravention { get; set; }
        public string? userid { get; set; }
    }

    public class GETWorkFlowHistory
    {
        public string? search { get; set; }
        public string? SearchADV { get; set; }
        public string? formno { get; set; }
        public string Plant { get; set; }
    }
}

