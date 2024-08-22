using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts.CAR
{
    public interface IIssueSubmissionService
    {
        Task<ApiResponse<IEnumerable<string>>> GetIssueStatus(string FormNo);
    }
}
