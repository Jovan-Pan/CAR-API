using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.MasterData
{
    public class ExcelReadResponseDto
    {
        public DataTable? DataTable { get; set; }
        public string? Message { get; set; }
        public bool Success => DataTable != null && DataTable.Rows.Count > 0;
    }
}
