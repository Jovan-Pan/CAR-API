using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ParamRequest
{
    public class IssueFeedbacReportUpdateParam
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int UserPlant { get; set; }
        public string StatusOfFinding { get; set; }
        public string FormNo { get; set; }
    }
}
