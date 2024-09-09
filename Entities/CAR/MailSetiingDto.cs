using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.CAR
{
    public class MailSetiingDto
    {
        public int id{ get; set; }
        public int plant { get; set; }
        public string? actionType { get; set; }
        public string? actionTypeDesc { get; set; }
        public bool isSendEmail { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? updatedBy { get; set; }
        public string? updatedByName { get; set; }
        public DateTime? updatedDate { get; set; }

    }
}
