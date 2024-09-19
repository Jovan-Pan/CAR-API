using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ParamRequest
{
    public class ConditionParams
    {
        public int take { get; set; }
        public int skip { get; set; }
        public string ExtraWhereCondition { get; set; }
        public string OrderByCondition { get; set; }
    }
}
