using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ParamRequest
{
    public class GlobalSettingParam
    {
        public int Plant { get; set; }
        public string SettingID { get; set; }
        public string System { get; set; } = "CAR";
    }
}
