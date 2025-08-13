using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.MasterData
{
    public class SystemvsUservsEmailSubscribeForm
    {
        public int Plant { get; set; }
        public string SystemCode { get; set; }
        public string? Dept { get; set; }
        public string UserID { get; set; }
        public string Group { get; set; }
        public string CategoryName { get; set; }
        public string UseEmail { get; set; }
        public string? UseNam { get; set; }
    }
}
