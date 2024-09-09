using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.CAR
{
    public class TotalRecordForEachSttsDto
    {
        public int Submitted { get; set; }
        public int Open { get; set; }
        public int PdaDecision { get; set; }
        public int issued { get; set; }
        public int ActIssued { get; set; }
        public int Analize { get; set; }
        public int review { get; set; }
        public int Complete { get; set; }
    }
}
