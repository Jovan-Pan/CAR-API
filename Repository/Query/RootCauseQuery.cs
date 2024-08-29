using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Query
{
    public class RootCauseQuery
    {
        public static readonly string getRootCauseList = @"
        select distinct RootCauseName from RootCauseCategory
        where DelFlag = 0 and Plant = @plant";
    }
}
