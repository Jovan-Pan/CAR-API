using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.CAR
{
    public class IssueFeedbackAtchmentDto
    {
        public string FormNo { get; set; }
        public string ActionType { get; set; }
        public string OriFileName { get; set; }
        public string FileName { get; set; }
        public string FileExt { get; set; }
        public string FilePath { get; set; }
    }
}
