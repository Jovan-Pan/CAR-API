using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ParamRequest
{
    public class GetMatGrpParam
    {
        public int plant { get; set; }
        public string? product { get; set; }
        public IEnumerable<string>? productAuthList { get; set; }
    }
}
