using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.MasterData
{
    public class ProcessDto
    {
        public int ID { get; set; }
        public string Plant { get; set; }
        public string Process { get; set; }
        public bool DelFlag { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedByName { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class CRUDProcessDto
    {
        public int? id { get; set; }
        public string? Plant { get; set; }
        public string? Process { get; set; }
        public string? userid { get; set; }
    }

    public class GETProcess
    {
        public string? search { get; set; }
        public string? SearchADV { get; set; }
        public bool delflag { get; set; }
        public string? plant { get; set; }
    }
}
