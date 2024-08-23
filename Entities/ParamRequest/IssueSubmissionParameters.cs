using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ParamRequest
{
    public class IssueSubmissionParameters
    {
        //public IEnumerable<IFormFile>? NCCategoryImgFiles { get; set; }
        //public IEnumerable<IFormFile>? NCCategoryFiles { get; set; }

        //public IEnumerable<IFormFile>? immediteActReceiverImgFiles { get; set; }
        //public IEnumerable<IFormFile>? immediteActReceiverFiles { get; set; }

        //public IEnumerable<IFormFile>? rootCauseReceiverImgFiles { get; set; }
        //public IEnumerable<IFormFile>? rootCauseReceiverFiles { get; set; }

        //public IEnumerable<IFormFile>? correctiveActReceiverImgFiles { get; set; }
        //public IEnumerable<IFormFile>? correctiveActReceiverFiles { get; set; }

        //public IEnumerable<IFormFile>? reviewerImgFiles { get; set; }
        //public IEnumerable<IFormFile>? reviewerFiles { get; set; }

        public DateTime? DetectionDate { get; set; }
        public string? Product { get; set; }
        public string? Model { get; set; }
        public string? MaterialType { get; set; }
        public string? MaterialCode { get; set; }
        public int? SamplingCheck { get; set; }
        public string? Dept { get; set; }
        public string? VendorCode { get; set; }
        public string? VendorDesc { get; set; }
        public decimal? TttlQty { get; set; }
        public string? TttlQtyUOM { get; set; }
        public int? AffectedCavity { get; set; }
        public string? IssueType { get; set; }
        public string? NCCode { get; set; }
        public string? NCCategory { get; set; }
        public string? NCReason { get; set; }
        public string? NCDescription { get; set; }
        public string? Comment { get; set; }
        public string UserId { get; set; }
        public int UserPlant { get; set; }
    }
}
