using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ParamRequest
{
    public class GetMaterialParam
    {
        public int plant { get; set; }
        public string? product { get; set; }
        public IEnumerable<string>? productAuthList { get; set; }
        public string? matgroup { get; set; }
        public string? mattype { get; set; }
        public string? searchTerm { get; set; }
    }
}
