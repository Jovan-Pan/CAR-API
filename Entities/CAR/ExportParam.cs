using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.CAR
{
    public class ExportParam
    {
        public int? no { get; set; }
        public int? plant { get; set; }
        public string? actionType { get; set; }
        public string? actionTypeDesc { get; set; }
        public bool? isSendEmail { get; set; }
        public bool? isDeleted { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public string? CreatedDate { get; set; }
        public string? updatedBy { get; set; }
        public string? updatedByName { get; set; }
        public string? updatedDate { get; set; }
        public string? search { get; set; }
        public string? ATsearchADV { get; set; }
        public string? ATDsearchADV { get; set; }
        public string? globalSearch { get; set; }
        public string? CreatedDateInput { get; set; }
        public string? updatedDateInput { get; set; }
    }
}
