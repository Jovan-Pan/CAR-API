using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ParamRequest
{
    public class IssueSbmsAtchParam
    {
        public IEnumerable<IFormFile>? NCCategoryImgFiles { get; set; }
        public IEnumerable<IFormFile>? NCCategoryFiles { get; set; }

        public IEnumerable<IFormFile>? immediteActReceiverImgFiles { get; set; }
        public IEnumerable<IFormFile>? immediteActReceiverFiles { get; set; }

        public IEnumerable<IFormFile>? rootCauseReceiverImgFiles { get; set; }
        public IEnumerable<IFormFile>? rootCauseReceiverFiles { get; set; }

        public IEnumerable<IFormFile>? correctiveActReceiverImgFiles { get; set; }
        public IEnumerable<IFormFile>? correctiveActReceiverFiles { get; set; }

        public IEnumerable<IFormFile>? reviewerImgFiles { get; set; }
        public IEnumerable<IFormFile>? reviewerFiles { get; set; }
    }
}
