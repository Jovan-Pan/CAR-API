using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Query
{
    public class ImmidateActionQuery
    {
        public static readonly string getImmidateActionList = @"
        select distinct ImmidateName from ImmidateAction
        where DelFlag = 0 and Plant = @plant";
    }
}
