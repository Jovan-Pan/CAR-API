using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ParamRequest
{
    public class GlobalParam: BaseRequestParameters
    {
        public int Plant {  get; set; }
        public IEnumerable<string>? deptAuthList { get; set; }
        public IEnumerable<string>? productAuthList { get; set; }
        public IEnumerable<string>? formType { get; set; }
        public IEnumerable<string>? formNumber { get; set; }
        public IEnumerable<string>? status { get; set; }
        public IEnumerable<string>? dept { get; set; }
        public IEnumerable<string>? processGrp { get; set; }
        public IEnumerable<string>? product { get; set; }
        public IEnumerable<string>? model { get; set; }
        public IEnumerable<string>? mattype { get; set; }
        public IEnumerable<string>? material { get; set; }
        public IEnumerable<string>? vendor { get; set; }
        public string? datetype { get; set; }
        public DateTime? fromdate { get; set; }
        public DateTime? todate { get; set; }

    }
}
