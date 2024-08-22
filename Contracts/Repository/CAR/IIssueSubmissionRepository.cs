using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Repository.CAR
{
    public interface IIssueSubmissionRepository
    {
        Task<IEnumerable<string>> GetIssueStatus(string FormNo);
    }
}
