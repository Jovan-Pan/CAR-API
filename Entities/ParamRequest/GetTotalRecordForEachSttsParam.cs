using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ParamRequest
{
    public class GetTotalRecordForEachSttsParam
    {
        public int plant { get; set; }
        public IEnumerable<string> DeptList { get; set; }
        public IEnumerable<string> ProductList { get; set; }
    }
}
