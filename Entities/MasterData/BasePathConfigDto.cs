using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.MasterData
{
    public class BasePathConfigDto
    {
        public int plant {  get; set; }
        public string system { get; set; }
        public string domain { get; set; }
        public string userID { get; set; }
        public string password { get; set; }
        public string basePath { get; set; }
    }
}
