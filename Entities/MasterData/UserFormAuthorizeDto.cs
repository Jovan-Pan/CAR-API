using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.MasterData
{
    public class UserFormAuthorizeDto
    {
        public string FormName { get; set; }
        public string formdesc { get; set; }
        public bool IsUserAuViewOnly { get; set; }
        public bool ViewOnly { get; set; }
    }
}
