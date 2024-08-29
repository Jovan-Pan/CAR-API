using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.CAR
{
    public class DirectoryCredentials
    {
        public string Domain { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public string BasePath { get; set; }
        public bool Success { get; set; } = false;
        public string? Message { get; set; }
    }
}
