using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.CAR
{
    public class PagedResponse
    {
        public PagedList<dynamic> data { get; set; }
        public List<dynamic> datadet { get; set; }
        public MetaData metaData { get; set; }
        public bool success { get; set; } = true;
        public string message { get; set; }
    }
}
