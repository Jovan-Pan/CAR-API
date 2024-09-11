using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.MasterData
{
    public class RootCauseCategoryDto
    {
        public int id { get; set; }
        public string RootCauseName { get; set; }
        public int Plant { get; set; } 
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } 
        public string? UpdatedBy { get; set; } 
        public DateTime? UpdatedDate { get; set; }
        public bool DelFlag { get; set; }

    }
}
