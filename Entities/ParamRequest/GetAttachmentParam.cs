using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ParamRequest
{
    public class GetAttachmentParam
    {
        public int Plant {  get; set; }
        public string filepath {  get; set; }
        public string filename { get; set; }
        public string fileExt { get; set; }
    }
}
