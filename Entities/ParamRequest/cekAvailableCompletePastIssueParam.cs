using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ParamRequest
{
    public class cekAvailableCompletePastIssueParam
    {
        public int plant { get; set; }
        public string? material { get; set; }
        public string ncreason { get; set; }
        public string nccategory { get; set; }
        public string dept { get; set; }
        public string? vendor { get; set; }
        public string processgroup { get; set; }

        public int SetFormTypeStatusRange { get; set; }
    }
}
