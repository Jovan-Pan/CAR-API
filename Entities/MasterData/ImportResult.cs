using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.MasterData
{
    public class ImportResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<Dictionary<string, object>> DataInvalid { get; set; }
        public int? DataValidCount { get; set; }
        public int ?DataInvalidCount { get; set; }
        public string? Label { get; set; }
        public string? result { get; set; }
    }
}
