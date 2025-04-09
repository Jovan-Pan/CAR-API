using Entities.MasterData;
using Entities.ParamRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.CAR
{
    public class IssueFeedbackResultDto
    {
        public IEnumerable<IssueFeedbackDto> maindata { get; set; }
        public IEnumerable<IssueFeedbackAtchmentDto> dataAtch { get; set; }
        public IssueSbmsAtchParam formFiles { get; set; }
        public int totrecord { get; set; }
      
        public IEnumerable<UsrDto>? EmailRecipientsList { get; set; }
    }
}
