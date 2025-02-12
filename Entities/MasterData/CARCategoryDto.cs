using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.MasterData
{
    public class CARCategoryDto
    {
        public int id { get; set; }
        public int Plant { get; set; }
        public string CARCategory { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedByName { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool DelFlag { get; set; }
    }
    public class CRUDCARCategoryDto
    {
        public int? id { get; set; }
        public int? Plant { get; set; }
        public string? CARCategory { get; set; }
        public string? userid { get; set; }
    }
}
