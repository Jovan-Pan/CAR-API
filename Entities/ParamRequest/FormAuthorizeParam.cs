using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ParamRequest
{
    public class FormAuthorizeParam
    {
        public int plant { get; set; }
        public string? userId { get; set; }
        public IEnumerable<string>? FormId { get; set; }
    }
}
