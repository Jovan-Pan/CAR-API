using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ParamRequest
{
    public class SendMailSettingParam
    {
        public int? id { get; set; }
        public int plant { get; set; }
        public string? ActionType { get; set; }
        public string? ActionTypeDesc { get; set; }
        public bool isSendEmail { get; set; }
        public string? userId { get; set; }
        public string? userName { get; set; }
    }
}
