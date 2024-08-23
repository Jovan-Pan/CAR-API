using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.CAR
{
    public class ErrorLogModel
    {
        public int? ID { get; set; }
        public string? ErrDescription { get; set; }
        public string? ErrType { get; set; }
        public string? ErrSource { get; set; }
        public string? ErrUrl { get; set; }

        public string? AddedBy { get; set; }
        public DateTime? AddedOn { get; set; }
    }
}
