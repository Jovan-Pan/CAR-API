using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.MasterData
{
    public class TGlobalEmailSettingModel
    {
        public string? SysCode { get; set; }
        public int? Plant { get; set; }
        public string? EmailSubject { get; set; }
        public string? Emailbody { get; set; }
        public string? EmailFooter { get; set; }
        public string? Emaillink { get; set; }
        public string? ReplyMailid { get; set; }
        public string? HLevel { get; set; }
        public string? FromMailaddress { get; set; }
        public string? WStatus { get; set; }
    }
}
