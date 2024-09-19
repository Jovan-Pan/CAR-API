using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Account.Dto
{
    public class FormAuthorizeInfoDto
    {
        public bool isuserAuViewOnly { get; set; }
        public bool viewOnly { get; set; }
        public string formdesc { get; set; }
    }
}
